using ST.Checking.Abstraction;
namespace ST.Checking;
public abstract class BaseModuleChecker<TContext> : IModuleChecker where TContext : class, IModuleCheckingContext
{
    public bool CanCheck(IModuleCheckingContext context) => context is TContext;
    public void Check(IModuleCheckingContext context) => InternalCheck((context as TContext)!);
    protected abstract void InternalCheck(TContext context);
}