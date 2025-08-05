namespace ST.Validating.Abstraction;
public interface IModuleValidator
{
    bool CanValidate(IModuleValidationContext context);
    bool Validate(IModuleValidationContext context);
}