using ModelContextProtocol.Server;
using System.ComponentModel;

namespace CorePlatformMcp;

// TODO (squad): this class is the home for your tool(s). The DI wiring is done —
// _api is ready to use. Your job is the [McpServerTool] method below.
//
// Reminder from the lab: write the [Description] before you write the implementation.
// If you can't say in one sentence what this tool does and when Claude should call it,
// stop and talk it through with your squad first.
[McpServerToolType]
public class RatesTools
{
    private readonly IRatesApiClient _api;

    public RatesTools(IRatesApiClient api) => _api = api;

    // Delete this comment block and write your tool here. Your starting signature
    // (from the lab README) is:
    //
    //     GetRateAvailability(propertyId, dateRange)
    //
    // "dateRange" is deliberately vague in the lab brief — that's your design decision.
    // The stub client takes two DateOnly values (checkIn/checkOut). You could expose the
    // tool the same way, or take a single string like "2026-07-01..2026-07-05" and parse
    // it — discuss which is easier for Claude to construct reliably from a guest message.
    //
    // Example shape to follow — uncomment and adapt, don't just fill in blindly:
    //
    // [McpServerTool]
    // [Description("One sentence: what this tool does. One sentence: when Claude should " +
    //     "call it. One sentence: any important constraints or caveats.")]
    // public async Task<IReadOnlyList<RateNight>> GetRateAvailability(
    //     [Description("The eviivo property ID, e.g. PROP-04821")]
    //     string propertyId,
    //     [Description("First night of the stay, inclusive, e.g. 2026-07-01")]
    //     DateOnly checkIn,
    //     [Description("Departure date, exclusive — the stay covers checkIn up to but not " +
    //         "including this date, e.g. 2026-07-05")]
    //     DateOnly checkOut)
    // {
    //     return await _api.GetRateAvailabilityAsync(propertyId, checkIn, checkOut);
    // }
}
