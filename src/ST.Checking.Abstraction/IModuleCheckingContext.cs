namespace ST.Checking.Abstraction;
public interface IModuleCheckingContext
{
    float Score   { get; set; }
    bool  Checked { get; set; }
}