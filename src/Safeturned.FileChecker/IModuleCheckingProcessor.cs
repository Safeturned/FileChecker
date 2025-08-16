namespace Safeturned.FileChecker;
public interface IModuleCheckingProcessor
{
    IModuleProcessingContext Process(Stream stream);
}