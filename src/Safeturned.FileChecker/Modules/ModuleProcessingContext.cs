using dnlib.DotNet;

namespace Safeturned.FileChecker.Modules;

internal class ModuleProcessingContext : IModuleProcessingContext
{
    private readonly List<string> _analysisResults = new();

    public ModuleProcessingContext(Stream sourceStream, ModuleDefMD module)
    {
        Module = module;
        SourceStream = sourceStream;
    }

    public ModuleDefMD Module { get; }
    public Stream SourceStream { get; }
    public float Score { get; set; }
    public string? Message { get; set; }
    public bool Checked { get; set; }

    public void AddAnalysisResult(string result)
    {
        _analysisResults.Add(result);
    }

    public object GetAnalysisResults()
    {
        return new
        {
            message = Message,
            results = _analysisResults.ToArray()
        };
    }
}