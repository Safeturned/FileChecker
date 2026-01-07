namespace Safeturned.FileChecker.Modules;

internal interface IModuleAnalyzer
{
    string FeatureName { get; }
    FeatureResult Analyze(ModuleProcessingContext context);
}