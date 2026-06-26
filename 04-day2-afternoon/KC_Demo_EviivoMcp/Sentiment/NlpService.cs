namespace EviivoMcp;

/// <summary>
/// Stub implementation of INlpService.
/// Replace the body of AnalyseSentiment with a real HTTP call to your NLP endpoint,
/// e.g. Azure AI Language, AWS Comprehend, or a custom model API.
/// </summary>
public class NlpService : INlpService
{
    public Task<SentimentResult> AnalyseSentiment(string text)
    {
        // --- Stub logic: keyword-based heuristic ---
        var lower = text.ToLowerInvariant();

        var (sentiment, score) = lower.ContainsAny("terrible", "awful", "furious", "disgusting", "worst", "unacceptable")
            ? ("negative", 0.92)
            : lower.ContainsAny("unhappy", "disappointed", "frustrated", "annoyed", "bad", "problem", "issue", "wrong")
            ? ("negative", 0.70)
            : lower.ContainsAny("great", "excellent", "amazing", "wonderful", "fantastic", "love", "perfect", "happy")
            ? ("positive", 0.88)
            : ("neutral", 0.55);

        var urgency = lower.ContainsAny("urgent", "immediately", "asap", "emergency", "now", "right away", "critical")
            ? "high"
            : sentiment == "negative" && score >= 0.85
            ? "high"
            : sentiment == "negative"
            ? "medium"
            : "low";

        return Task.FromResult(new SentimentResult(sentiment, score, urgency));
    }
}

internal static class StringExtensions
{
    internal static bool ContainsAny(this string source, params string[] values)
        => values.Any(v => source.Contains(v, StringComparison.OrdinalIgnoreCase));
}
