using dnlib.DotNet;

namespace Safeturned.FileChecker.Modules;

internal class ModuleProcessingContext : IModuleProcessingContext
{
    private readonly List<FeatureResult> _features = [];

    public ModuleProcessingContext(Stream sourceStream, ModuleDefMD module)
    {
        Module = module;
        SourceStream = sourceStream;
    }

    public ModuleDefMD Module { get; }
    public Stream SourceStream { get; }
    public float Score => _features.Sum(f => f.Score);
    public IReadOnlyList<FeatureResult> Features => _features;

    public void AddFeatureResult(FeatureResult feature)
    {
        _features.Add(feature);
    }
}