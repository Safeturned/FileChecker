using ST.Validating.Abstraction;
namespace ST.Validating;
public abstract class BaseModuleValidator<TContext> : IModuleValidator where TContext : class, IModuleValidationContext
{
    public bool CanValidate(IModuleValidationContext context) => context is TContext;
    public bool Validate(IModuleValidationContext context) => InternalValidate((context as TContext)!);
    protected abstract bool InternalValidate(TContext context);
}