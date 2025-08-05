using DI.Engine.Attributes;
using DI.Services.Abstraction;
using DI.Services.Scheme.Abstraction;
using DI.Services.Scheme.Attributes;
using dnlib.DotNet;
using ST.Checking.Abstraction;
using ST.Checking.DnLib;
using ST.CheckingProcessor.Abstraction;
using System.Collections.Generic;
using System.IO;
namespace ST.CheckingProcessor.DnLib;
[DiDescript(Order = 10, Lifetime = EDiServiceLifetime.Singleton, ServiceType = typeof(IModuleCheckingProcessor))]
public class DnLibModuleCheckingProcessor(
    [DiParameter(IgnoreKeys = true, ServiceEquals = EDiServiceEquals.SubClassOrAssignableOrEquals)] IEnumerable<IModuleChecker> moduleCheckers)
    : IModuleCheckingProcessor
{
    public IModuleCheckingContext Process(Stream stream)
    {
        ModuleDefMD               module  = ModuleDefMD.Load(stream);
        DnLibModuleCheckerContext context = new(module);
        foreach(IModuleChecker moduleChecker in moduleCheckers)
        {
            if(!moduleChecker.CanCheck(context)) continue;

            moduleChecker.Check(context);
        }
        return context;
    }
}