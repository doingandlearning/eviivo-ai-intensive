namespace EviivoMcp;

/// <summary>Sentiment analysis result returned by the NLP service.</summary>
/// <param name="Sentiment">Overall sentiment: "positive", "neutral", or "negative".</param>
/// <param name="Score">Confidence score in the range 0.0–1.0.</param>
/// <param name="Urgency">Detected urgency level: "low", "medium", or "high".</param>
public record SentimentResult(
    string Sentiment,
    double Score,
    string Urgency
);
