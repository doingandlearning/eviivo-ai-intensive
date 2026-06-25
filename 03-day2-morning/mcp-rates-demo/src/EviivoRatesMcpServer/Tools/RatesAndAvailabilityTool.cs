using System.ComponentModel;
using EviivoRatesMcpServer.Models;
using EviivoRatesMcpServer.Services;
using ModelContextProtocol.Server;

namespace EviivoRatesMcpServer.Tools;

/// <summary>
/// The "good description" tool — this is the one the demo runs by default
/// (<c>DemoMode:UseVagueToolDescriptions = false</c> in appsettings.json).
///
/// The tool description below is copied verbatim from "Description B" in the Day 2 morning
/// session's Exercise 2 (day2-morning/README.md), and the method signature matches that
/// session's own "MCP server design template" C# slide exactly — same method name, same
/// parameter names and order, same XML-doc-via-[Description] pattern. Attendees should
/// recognise this tool as the worked answer to the exercise they just did.
///
/// See <see cref="RatesAndAvailabilityToolVague"/> for the deliberately-bad twin used to
/// demonstrate the failure mode. Program.cs registers exactly one of the two — both define
/// a tool that resolves to the same name (<c>get_rates_and_availability</c>), so registering
/// both at once would be a duplicate-tool-name conflict, not a meaningful comparison.
/// </summary>
[McpServerToolType]
public class RatesAndAvailabilityTool
{
    private readonly IRatesGateway _ratesGateway;

    public RatesAndAvailabilityTool(IRatesGateway ratesGateway)
    {
        _ratesGateway = ratesGateway;
    }

    [McpServerTool]
    [Description("Returns current rates and room availability for a property across a date range. " +
                 "Use this when you need to assess pricing, occupancy, or yield opportunities. " +
                 "Returns rate tiers, channel restrictions, and available room count per night.")]
    public async Task<RatesAvailabilityResult> GetRatesAndAvailability(
        [Description("The eviivo property identifier")] string propertyId,
        [Description("Start of the date range (inclusive), ISO 8601 format")] DateOnly dateFrom,
        [Description("End of the date range (inclusive), ISO 8601 format")] DateOnly dateTo,
        [Description("If true, returns rates broken down by OTA channel. Default: false.")] bool includeChannelRates = false)
    {
        // _ratesGateway stands in for eviivo's existing Rates/Availability service — see
        // Services/IRatesGateway.cs. Swapping the in-memory implementation for a real one in
        // Program.cs is the only change needed to point this at production data; nothing here
        // would need to change.
        var result = await _ratesGateway.GetRatesAndAvailabilityAsync(propertyId, dateFrom, dateTo, includeChannelRates);
        return RatesAvailabilityResult.FromGatewayResponse(result);
    }
}
