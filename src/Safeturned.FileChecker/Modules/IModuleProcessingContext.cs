namespace Safeturned.FileChecker.Modules;

public interface IModuleProcessingContext
{
    Stream SourceStream { get; }
    float Score { get; }
    IReadOnlyList<FeatureResult> Features { get; }
}