using dnlib.DotNet;
using System.IO;
namespace ST.CheckingProcessor.DnLib;
public class DnLibModuleProcessingContext(Stream sourceStream, ModuleDefMD module) : IDnLibModuleProcessingContext
{
    public ModuleDefMD Module       { get; } = module;
    public Stream      SourceStream { get; } = sourceStream;
    public float       Score        { get; set; }
    public string?     Message      { get; set; }
    public bool        Checked      { get; set; }
}