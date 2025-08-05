using ST.CheckingProcessor.Abstraction;
namespace ST.Validating.Abstraction;
public interface IModuleValidator
{
    bool CanValidate(IModuleProcessingContext context);
    bool Validate(IModuleProcessingContext context);
}