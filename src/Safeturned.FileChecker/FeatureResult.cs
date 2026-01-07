namespace Safeturned.FileChecker;

public record FeatureResult
{
    public string Name { get; init; } = string.Empty;
    public float Score { get; init; }
    public List<FeatureMessage>? Messages { get; init; }
}

public record FeatureMessage
{
    public string Text { get; init; } = string.Empty;
}
