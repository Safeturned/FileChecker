using dnlib.DotNet;
using ST.Validating.Abstraction;
namespace ST.Validating.DnLib;
public class DnLibModuleValidationContext(ModuleDefMD module) : IModuleValidationContext
{
    public float       Score  { get; set; }
    public ModuleDefMD Module { get; } = module;
}