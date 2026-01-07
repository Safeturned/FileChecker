using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Safeturned.FileChecker.Modules;

namespace Safeturned.FileChecker.Analyzers;

internal class NetworkActivityAnalyzer : IModuleAnalyzer
{
    public string FeatureName => "NetworkActivity";

    private static readonly HashSet<string> UploadMethods =
    [
        "System.Net.WebClient::UploadData",
        "System.Net.WebClient::UploadDataAsync",
        "System.Net.WebClient::UploadDataTaskAsync",
        "System.Net.WebClient::UploadFile",
        "System.Net.WebClient::UploadFileAsync",
        "System.Net.WebClient::UploadFileTaskAsync",
        "System.Net.WebClient::UploadString",
        "System.Net.WebClient::UploadStringAsync",
        "System.Net.WebClient::UploadStringTaskAsync",
        "System.Net.WebClient::UploadValues",
        "System.Net.WebClient::UploadValuesAsync",
        "System.Net.WebClient::UploadValuesTaskAsync",
        "System.Net.Http.HttpClient::PostAsync",
        "System.Net.Http.HttpClient::PutAsync",
        "System.Net.Http.HttpClient::SendAsync",
        "System.Net.Http.HttpContent::ReadAsByteArrayAsync",
        "UnityEngine.Networking.UnityWebRequest::Post",
        "UnityEngine.Networking.UnityWebRequest::Put",
        "UnityEngine.Networking.UploadHandlerRaw::.ctor",
        "UnityEngine.Networking.UploadHandler::.ctor",
        "UnityEngine.WWWForm::AddBinaryData",
        "UnityEngine.WWWForm::AddField",
    ];

    private static readonly HashSet<string> DownloadMethods =
    [
        "System.Net.WebClient::DownloadData",
        "System.Net.WebClient::DownloadDataAsync",
        "System.Net.WebClient::DownloadDataTaskAsync",
        "System.Net.WebClient::DownloadFile",
        "System.Net.WebClient::DownloadFileAsync",
        "System.Net.WebClient::DownloadFileTaskAsync",
        "System.Net.WebClient::DownloadString",
        "System.Net.WebClient::DownloadStringAsync",
        "System.Net.WebClient::DownloadStringTaskAsync",
        "System.Net.Http.HttpClient::GetAsync",
        "System.Net.Http.HttpClient::GetByteArrayAsync",
        "System.Net.Http.HttpClient::GetStringAsync",
        "System.Net.Http.HttpClient::GetStreamAsync",
        "UnityEngine.Networking.UnityWebRequest::Get",
        "UnityEngine.Networking.UnityWebRequest::SendWebRequest",
        "UnityEngine.Networking.UnityWebRequest::.ctor",
        "UnityEngine.Networking.DownloadHandlerBuffer::.ctor",
        "UnityEngine.Networking.DownloadHandlerFile::.ctor",
        "UnityEngine.Networking.DownloadHandler::.ctor",
        "UnityEngine.WWW::.ctor",
    ];

    private static readonly HashSet<string> FileReadMethods =
    [
        "System.IO.File::ReadAllBytes",
        "System.IO.File::ReadAllBytesAsync",
        "System.IO.File::ReadAllText",
        "System.IO.File::ReadAllTextAsync",
        "System.IO.File::ReadAllLines",
        "System.IO.File::ReadAllLinesAsync",
        "System.IO.File::OpenRead",
        "System.IO.File::Open",
        "System.IO.FileStream::.ctor",
    ];

    private static readonly HashSet<string> DirectoryEnumMethods =
    [
        "System.IO.Directory::GetFiles",
        "System.IO.Directory::EnumerateFiles",
        "System.IO.Directory::GetDirectories",
        "System.IO.Directory::EnumerateDirectories",
        "System.IO.DirectoryInfo::GetFiles",
        "System.IO.DirectoryInfo::EnumerateFiles",
    ];

    private static readonly HashSet<string> ReflectionEmitTypes =
    [
        "System.Reflection.Emit.AssemblyBuilder",
        "System.Reflection.Emit.ModuleBuilder",
        "System.Reflection.Emit.TypeBuilder",
        "System.Reflection.Emit.MethodBuilder",
        "System.Reflection.Emit.ILGenerator",
        "System.Reflection.Emit.DynamicMethod",
    ];

    private static readonly HashSet<string> Base64Methods =
    [
        "System.Convert::ToBase64String",
        "System.Convert::FromBase64String",
    ];

    private static readonly string[] SuspiciousPaths =
    [
        "plugins",
        "libraries",
        "rocket",
        "permissions.config",
        "rocket.config",
        ".dll",
    ];

    public FeatureResult Analyze(ModuleProcessingContext context)
    {
        var findings = new AnalysisFindings();

        foreach (var typeDef in context.Module.GetTypes())
        {
            AnalyzeType(typeDef, findings);
        }

        return CalculateScoreAndReport(findings);
    }

    private static void AnalyzeType(TypeDef typeDef, AnalysisFindings findings)
    {
        foreach (var method in typeDef.Methods)
        {
            if (!method.HasBody)
                continue;

            AnalyzeMethod(method, findings);
        }
    }

    private static void AnalyzeMethod(MethodDef method, AnalysisFindings findings)
    {
        foreach (var instruction in method.Body.Instructions)
        {
            if (instruction.OpCode != OpCodes.Call &&
                instruction.OpCode != OpCodes.Callvirt &&
                instruction.OpCode != OpCodes.Newobj)
                continue;

            var operand = instruction.Operand?.ToString();
            if (string.IsNullOrEmpty(operand))
                continue;

            CheckForNetworkMethods(operand, method, findings);
            CheckForFileSystemMethods(operand, method, findings);
            CheckForReflectionEmit(operand, method, findings);
            CheckForBase64(operand, method, findings);
            CheckForSuspiciousPaths(instruction, method, findings);
        }
    }

    private static void CheckForNetworkMethods(string operand, MethodDef method, AnalysisFindings findings)
    {
        foreach (var uploadMethod in UploadMethods)
        {
            if (operand.Contains(uploadMethod, StringComparison.OrdinalIgnoreCase))
            {
                findings.UploadMethodsFound.Add(new Finding(method.FullName, uploadMethod));
                return;
            }
        }

        foreach (var downloadMethod in DownloadMethods)
        {
            if (operand.Contains(downloadMethod, StringComparison.OrdinalIgnoreCase))
            {
                findings.DownloadMethodsFound.Add(new Finding(method.FullName, downloadMethod));
                return;
            }
        }
    }

    private static void CheckForFileSystemMethods(string operand, MethodDef method, AnalysisFindings findings)
    {
        foreach (var fileReadMethod in FileReadMethods)
        {
            if (operand.Contains(fileReadMethod, StringComparison.OrdinalIgnoreCase))
            {
                findings.FileReadMethodsFound.Add(new Finding(method.FullName, fileReadMethod));
                return;
            }
        }

        foreach (var dirEnumMethod in DirectoryEnumMethods)
        {
            if (operand.Contains(dirEnumMethod, StringComparison.OrdinalIgnoreCase))
            {
                findings.DirectoryEnumMethodsFound.Add(new Finding(method.FullName, dirEnumMethod));
                return;
            }
        }
    }

    private static void CheckForReflectionEmit(string operand, MethodDef method, AnalysisFindings findings)
    {
        foreach (var emitType in ReflectionEmitTypes)
        {
            if (operand.Contains(emitType, StringComparison.OrdinalIgnoreCase))
            {
                findings.ReflectionEmitFound.Add(new Finding(method.FullName, emitType));
                return;
            }
        }
    }

    private static void CheckForBase64(string operand, MethodDef method, AnalysisFindings findings)
    {
        foreach (var base64Method in Base64Methods)
        {
            if (operand.Contains(base64Method, StringComparison.OrdinalIgnoreCase))
            {
                findings.Base64MethodsFound.Add(new Finding(method.FullName, base64Method));
                return;
            }
        }
    }

    private static void CheckForSuspiciousPaths(Instruction instruction, MethodDef method, AnalysisFindings findings)
    {
        if (instruction.OpCode != OpCodes.Ldstr)
            return;

        var stringValue = instruction.Operand?.ToString();
        if (string.IsNullOrEmpty(stringValue))
            return;

        foreach (var suspiciousPath in SuspiciousPaths)
        {
            if (stringValue.Contains(suspiciousPath, StringComparison.OrdinalIgnoreCase))
            {
                findings.SuspiciousPathsFound.Add(new Finding(method.FullName, stringValue));
                break;
            }
        }
    }

    private FeatureResult CalculateScoreAndReport(AnalysisFindings findings)
    {
        float score = 0;
        var messages = new List<FeatureMessage>();
        var compoundThreats = new List<string>();

        if (findings.UploadMethodsFound.Count > 0)
        {
            score += 10;
            foreach (var finding in findings.UploadMethodsFound)
            {
                messages.Add(new FeatureMessage { Text = $"HTTP upload: {finding.Pattern} in {finding.Location}" });
            }
        }

        if (findings.DownloadMethodsFound.Count > 0)
        {
            score += 5;
            foreach (var finding in findings.DownloadMethodsFound)
            {
                messages.Add(new FeatureMessage { Text = $"HTTP download: {finding.Pattern} in {finding.Location}" });
            }
        }

        if (findings.FileReadMethodsFound.Count > 0)
        {
            score += 5;
            foreach (var finding in findings.FileReadMethodsFound)
            {
                messages.Add(new FeatureMessage { Text = $"File read: {finding.Pattern} in {finding.Location}" });
            }
        }

        if (findings.DirectoryEnumMethodsFound.Count > 0)
        {
            score += 10;
            foreach (var finding in findings.DirectoryEnumMethodsFound)
            {
                messages.Add(new FeatureMessage { Text = $"Directory enumeration: {finding.Pattern} in {finding.Location}" });
            }
        }

        if (findings.SuspiciousPathsFound.Count > 0)
        {
            score += 15;
            foreach (var finding in findings.SuspiciousPathsFound)
            {
                messages.Add(new FeatureMessage { Text = $"Suspicious path: '{finding.Pattern}' in {finding.Location}" });
            }
        }

        if (findings.ReflectionEmitFound.Count > 0)
        {
            score += 15;
            foreach (var finding in findings.ReflectionEmitFound)
            {
                messages.Add(new FeatureMessage { Text = $"Dynamic code generation: {finding.Pattern} in {finding.Location}" });
            }
        }

        if (findings.Base64MethodsFound.Count > 0)
        {
            score += 5;
            foreach (var finding in findings.Base64MethodsFound)
            {
                messages.Add(new FeatureMessage { Text = $"Base64: {finding.Pattern} in {finding.Location}" });
            }
        }

        score += CalculateCompoundBonuses(findings, compoundThreats);

        if (compoundThreats.Count > 0)
        {
            messages.Add(new FeatureMessage { Text = $"Compound threats: {string.Join(", ", compoundThreats)}" });
        }

        return new FeatureResult
        {
            Name = FeatureName,
            Score = score,
            Messages = messages.Count > 0 ? messages : null
        };
    }

    private static float CalculateCompoundBonuses(AnalysisFindings findings, List<string> threats)
    {
        float bonus = 0;

        var hasFileAccess = findings.FileReadMethodsFound.Count > 0 || findings.DirectoryEnumMethodsFound.Count > 0;
        var hasUpload = findings.UploadMethodsFound.Count > 0;
        var hasSuspiciousPaths = findings.SuspiciousPathsFound.Count > 0;
        var hasBase64 = findings.Base64MethodsFound.Count > 0;
        var hasDownload = findings.DownloadMethodsFound.Count > 0;
        var hasReflectionEmit = findings.ReflectionEmitFound.Count > 0;

        if (hasFileAccess && hasUpload)
        {
            bonus += 35;
            threats.Add("data exfiltration");
        }

        if (hasSuspiciousPaths && hasUpload)
        {
            bonus += 25;
            threats.Add("plugin stealing");
        }

        if (hasBase64 && hasUpload)
        {
            bonus += 15;
            threats.Add("encoded exfiltration");
        }

        if (hasDownload && hasReflectionEmit)
        {
            bonus += 20;
            threats.Add("payload loading");
        }

        if (findings.DirectoryEnumMethodsFound.Any(f => f.Pattern.Contains("GetFiles", StringComparison.OrdinalIgnoreCase)) &&
            findings.SuspiciousPathsFound.Any(f => f.Pattern.Contains(".dll", StringComparison.OrdinalIgnoreCase)))
        {
            bonus += 20;
            threats.Add("plugin enumeration");
        }

        return bonus;
    }

    private sealed class AnalysisFindings
    {
        public List<Finding> UploadMethodsFound { get; } = [];
        public List<Finding> DownloadMethodsFound { get; } = [];
        public List<Finding> FileReadMethodsFound { get; } = [];
        public List<Finding> DirectoryEnumMethodsFound { get; } = [];
        public List<Finding> SuspiciousPathsFound { get; } = [];
        public List<Finding> ReflectionEmitFound { get; } = [];
        public List<Finding> Base64MethodsFound { get; } = [];
    }

    private sealed record Finding(string Location, string Pattern);
}
