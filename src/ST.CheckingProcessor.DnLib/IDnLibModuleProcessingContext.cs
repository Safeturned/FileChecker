using dnlib.DotNet;
using ST.CheckingProcessor.Abstraction;
namespace ST.CheckingProcessor.DnLib;
public interface IDnLibModuleProcessingContext : IModuleProcessingContext
{
    ModuleDefMD Module { get; }
}