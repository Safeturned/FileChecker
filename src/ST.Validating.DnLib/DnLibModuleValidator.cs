using DI.Services.Scheme.Abstraction;
using DI.Services.Scheme.Attributes;
using ST.Validating.Abstraction;
using System.Linq;
namespace ST.Validating.DnLib;
[DiDescript(Order = 0, Lifetime = EDiServiceLifetime.Singleton, ServiceType = typeof(IModuleValidator), Key = "DnLibModuleValidator")]
public class DnLibModuleValidator : BaseModuleValidator<DnLibModuleValidationContext>
{
    protected override bool InternalValidate(DnLibModuleValidationContext context) =>
        context.Module.GetAssemblyRefs().Any(a => a.FullName.ToString().Contains("assembly-csharp"));
}