using DI.Services.Scheme.Abstraction;
using DI.Services.Scheme.Attributes;
using ST.CheckingProcessor.DnLib;
using ST.Validating.Abstraction;
using System.Linq;
namespace ST.Validating.DnLib;
[DiDescript(Order = 0, Lifetime = EDiServiceLifetime.Singleton, ServiceType = typeof(IModuleValidator), Key = "DnLibModuleDependenciesValidator")]
public class DnLibModuleDependenciesValidator : BaseModuleValidator<IDnLibModuleProcessingContext>
{
    protected override bool InternalValidate(IDnLibModuleProcessingContext context) =>
        context.Module.GetAssemblyRefs().Any(a => a.FullName.ToString().Contains("assembly-csharp"));
}