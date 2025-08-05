using ST.CheckingProcessor.Abstraction;
namespace ST.Checking.Abstraction;
public interface IModuleChecker
{
    bool CanCheck(IModuleProcessingContext context);
    void Check(IModuleProcessingContext context);
}