namespace EviivoMcp;

public interface INlpService
{
    /// <summary>Analyses the sentiment of the provided guest message text.</summary>
    Task<SentimentResult> AnalyseSentiment(string text);
}
