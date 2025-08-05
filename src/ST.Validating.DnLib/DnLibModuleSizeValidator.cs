using DI.Services.Scheme.Abstraction;
using DI.Services.Scheme.Attributes;
using ST.CheckingProcessor.DnLib;
using ST.Validating.Abstraction;
namespace ST.Validating.DnLib;
[DiDescript(Order = 0, Lifetime = EDiServiceLifetime.Singleton, ServiceType = typeof(IModuleValidator), Key = "DnLibModuleSizeValidator")]
public class DnLibModuleSizeValidator : BaseModuleValidator<IDnLibModuleProcessingContext>
{
    protected override bool InternalValidate(IDnLibModuleProcessingContext context) => context.SourceStream.Length < 512_000;
}