namespace Safeturned.FileChecker;
public interface IModuleProcessingContext
{
    Stream SourceStream { get; }
    float Score { get; set; }
    string? Message { get; set; }
    bool Checked { get; set; }
}