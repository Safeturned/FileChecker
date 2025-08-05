using ST.Checking.Abstraction;
using ST.CheckingProcessor.Abstraction;
namespace ST.Checking;
public abstract class BaseModuleChecker<TContext> : IModuleChecker where TContext : class, IModuleProcessingContext
{
    public bool CanCheck(IModuleProcessingContext context) => context is TContext;
    public void Check(IModuleProcessingContext context) => InternalCheck((context as TContext)!);
    protected abstract void InternalCheck(TContext context);
}