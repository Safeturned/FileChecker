namespace ST.Checking.Abstraction;
public interface IModuleChecker
{
    bool CanCheck(IModuleCheckingContext context);
    void Check(IModuleCheckingContext context);
}