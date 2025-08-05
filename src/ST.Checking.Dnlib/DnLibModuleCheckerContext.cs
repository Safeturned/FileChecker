using dnlib.DotNet;
using ST.Checking.Abstraction;
namespace ST.Checking.DnLib;
public class DnLibModuleCheckerContext(ModuleDefMD module) : IModuleCheckingContext
{
    public ModuleDefMD Module { get; } = module;
    public float       Score  { get; set; }
}