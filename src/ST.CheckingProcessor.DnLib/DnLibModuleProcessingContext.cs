using dnlib.DotNet;
using ST.CheckingProcessor.Abstraction;
using System.IO;
namespace ST.CheckingProcessor.DnLib;
public class DnLibModuleProcessingContext(Stream sourceStream, ModuleDefMD module) : IModuleProcessingContext
{
    public ModuleDefMD Module       { get; } = module;
    public Stream      SourceStream { get; } = sourceStream;
    public float       Score        { get; set; }
    public bool        Checked      { get; set; }
}