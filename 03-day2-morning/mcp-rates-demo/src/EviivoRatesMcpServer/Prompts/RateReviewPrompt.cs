using System.ComponentModel;
using ModelContextProtocol.Server;
using ModelContextProtocol.Protocol;

namespace EviivoRatesMcpServer.Prompts;

/// <summary>
/// Reusable prompt template for a structured rate review. Demonstrates the Prompts primitive:
/// a server-side task template the agent receives and executes, rather than a tool the agent
/// calls to get data.
///
/// This is the demo's illustration of "Prompts — standardised interactions" from the morning
/// session's three-primitives slide. The description tells the client when to surface it;
/// the messages tell the agent exactly what steps to follow when it does.
/// </summary>
[McpServerPromptType]
public class RateReviewPrompt
{
    [McpServerPrompt(Name = "review-rates")]
    [Description("Structured rate review for a property over a date range. " +
                 "Use when asked to assess pricing health, occupancy, or yield opportunities. " +
                 "Fetches live rates, loads property config, and returns a concise recommendation.")]
    public static IList<PromptMessage> ReviewRates(
        [Description("The eviivo property identifier, e.g. PROP-04821")] string propertyId,
        [Description("Start of the review period (inclusive), ISO 8601 date, e.g. 2026-07-17")] string dateFrom,
        [Description("End of the review period (inclusive), ISO 8601 date, e.g. 2026-07-20")] string dateTo)
    {
        return
        [
            new PromptMessage
            {
                Role = Role.User,
                Content = new TextContentBlock
                {
                    Text = $"""
                        Review rates and availability for property {propertyId} from {dateFrom} to {dateTo}.

                        Steps:
                        1. Load the property configuration from eviivo://property/{propertyId}/config
                           so you have the yield target, min-stay rules, and stop-sell policy.
                        2. Call get_rates_and_availability with propertyId={propertyId},
                           dateFrom={dateFrom}, dateTo={dateTo}, includeChannelRates=true.
                        3. Summarise the results in this structure:
                           - Average nightly base rate (GBP)
                           - Lowest availability night (date + rooms remaining)
                           - Any stop-sell nights (dates)
                           - Whether the occupancy trend is on track against the yield target
                           - One concrete pricing recommendation

                        Keep the summary under 150 words.
                        """
                }
            }
        ];
    }
}
