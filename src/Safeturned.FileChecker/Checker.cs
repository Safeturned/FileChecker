using System.Reflection;
using dnlib.DotNet;
using Safeturned.FileChecker.Analyzers;
using Safeturned.FileChecker.Modules;

namespace Safeturned.FileChecker;

public static class Checker
{
    public static string Version { get; } = Assembly.GetExecutingAssembly()
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        ?.InformationalVersion
        ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString()
        ?? "0.0.0.0";

    public static IModuleProcessingContext Process(Stream stream)
    {
        var module = ModuleDefMD.Load(stream);
        var context = new ModuleProcessingContext(stream, module);
        List<IModuleAnalyzer> analyzers =
        [
            new BlacklistedCommandAnalyzer(),
            new NetworkActivityAnalyzer(),
        ];
        foreach (var moduleAnalyzer in analyzers)
        {
            var featureResult = moduleAnalyzer.Analyze(context);
            context.AddFeatureResult(featureResult);
        }
        return context;
    }
}