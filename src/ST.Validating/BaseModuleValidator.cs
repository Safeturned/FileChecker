using ST.CheckingProcessor.Abstraction;
using ST.Validating.Abstraction;
namespace ST.Validating;
public abstract class BaseModuleValidator<TContext> : IModuleValidator where TContext : class, IModuleProcessingContext
{
    public bool CanValidate(IModuleProcessingContext context) => context is TContext;
    public bool Validate(IModuleProcessingContext context) => InternalValidate((context as TContext)!);
    protected abstract bool InternalValidate(TContext context);
}