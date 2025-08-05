using System.IO;
namespace ST.CheckingProcessor.Abstraction;
public interface IModuleCheckingProcessor
{
    IModuleProcessingContext Process(Stream stream);
}