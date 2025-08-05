using ST.Checking.Abstraction;
using System.IO;
namespace ST.CheckingProcessor.Abstraction;
public interface IModuleCheckingProcessor
{
    IModuleCheckingContext Process(Stream stream);
}