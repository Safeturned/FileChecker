using System.IO;
namespace ST.CheckingProcessor.Abstraction;
public interface IModuleProcessingContext
{
    Stream SourceStream { get; }
    float  Score        { get; set; }
    bool   Checked      { get; set; }
}