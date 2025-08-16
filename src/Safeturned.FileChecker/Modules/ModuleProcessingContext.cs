using dnlib.DotNet;

namespace Safeturned.FileChecker.Modules;

internal class ModuleProcessingContext : IModuleProcessingContext
{
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
}