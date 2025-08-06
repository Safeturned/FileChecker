using DI.Engine.Attributes;
using DI.Services.Abstraction;
using DI.Services.Scheme.Abstraction;
using DI.Services.Scheme.Attributes;
using dnlib.DotNet;
using ST.Checking.Abstraction;
using ST.CheckingProcessor.Abstraction;
using ST.Validating.Abstraction;
using System.Collections.Generic;
using System.IO;
namespace ST.CheckingProcessor.DnLib;
[DiDescript(Order = 10, Lifetime = EDiServiceLifetime.Singleton, ServiceType = typeof(IModuleCheckingProcessor))]
public class DnLibModuleCheckingProcessor(
    [DiParameter(IgnoreKeys = true, ServiceEquals = EDiServiceEquals.SubClassOrAssignableOrEquals)] IEnumerable<IModuleChecker> moduleCheckers,
    [DiParameter(IgnoreKeys = true, ServiceEquals = EDiServiceEquals.SubClassOrAssignableOrEquals)] IEnumerable<IModuleValidator> moduleValidators)
    : IModuleCheckingProcessor
{
    public IModuleProcessingContext Process(Stream stream)
    {
        ModuleDefMD                   module            = ModuleDefMD.Load(stream);
        IDnLibModuleProcessingContext processingContext = new DnLibModuleProcessingContext(stream, module);
        foreach(IModuleValidator moduleValidator in moduleValidators)
            if(moduleValidator.CanValidate(processingContext) && !moduleValidator.Validate(processingContext))
                return processingContext;

        foreach(IModuleChecker moduleChecker in moduleCheckers)
        {
            if(!moduleChecker.CanCheck(processingContext)) continue;

            moduleChecker.Check(processingContext);
        }
        processingContext.Checked = true;
        return processingContext;
    }
}