using DI.Services.Scheme.Abstraction;
using DI.Services.Scheme.Attributes;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ST.Checking.Abstraction;
namespace ST.Checking.DnLib;
[DiDescript(Order = 0, Lifetime = EDiServiceLifetime.Singleton, ServiceType = typeof(IModuleChecker), Key = "DnLibModuleChecker")]
public class DnLibModuleChecker : BaseModuleChecker<DnLibModuleCheckerContext>
{
    protected override void InternalCheck(DnLibModuleCheckerContext context)
    {
        foreach(TypeDef typeDef in context.Module.GetTypes())
        foreach(MethodDef typeDefMethod in typeDef.Methods)
        {
            if(!typeDefMethod.HasBody) continue;

            foreach(Instruction instruction in typeDefMethod.Body.Instructions)
            {
                // checks
            }
        }
    }
}