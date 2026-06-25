using EviivoMcp;
using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerToolType]
public class SentimentTools
{
    private readonly INlpService _nlp;

    public SentimentTools(INlpService nlp) => _nlp = nlp;

    [McpServerTool, Description(
        "Analyses the sentiment of a guest message and returns the overall tone " +
        "(positive, neutral, or negative), a confidence score, and an urgency level. " +
        "Use this before deciding how to escalate a guest complaint."
    )]
    public async Task<SentimentResult> AnalyseSentiment(
        [Description("The raw text of the guest's message or complaint to analyse.")]
        string text
    )
    {
        return await _nlp.AnalyseSentiment(text);
    }
}
