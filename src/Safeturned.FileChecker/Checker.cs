using System.Reflection;
using dnlib.DotNet;
using Safeturned.FileChecker.Analyzers;
using Safeturned.FileChecker.Modules;

namespace Safeturned.FileChecker;

public static class Checker
{
    public static string Version { get; } = Assembly.GetExecutingAssembly()
        .GetName()
        .Version?
        .ToString()!;

    public static IModuleProcessingContext Process(Stream stream)
    {
        var module = ModuleDefMD.Load(stream);
        var context = new ModuleProcessingContext(stream, module);
        List<IModuleAnalyzer> analyzers =
        [
            new BlacklistedCommandAnalyzer()
        ];
        foreach (var moduleAnalyzer in analyzers)
        {
            moduleAnalyzer.Analyze(context);
        }
        return context;
    }
}