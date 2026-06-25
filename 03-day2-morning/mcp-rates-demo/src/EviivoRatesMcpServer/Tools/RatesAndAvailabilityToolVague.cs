using System.ComponentModel;
using EviivoRatesMcpServer.Models;
using EviivoRatesMcpServer.Services;
using ModelContextProtocol.Server;

namespace EviivoRatesMcpServer.Tools;

/// <summary>
/// The "vague description" twin of <see cref="RatesAndAvailabilityTool"/> — same underlying
/// behaviour, same gateway, same structured result. The only thing that changes is the tool
/// description and the loss of parameter-level [Description] text. That's the whole point.
///
/// The description below is "Description A" from the Day 2 morning session's Exercise 2
/// (day2-morning/README.md) verbatim. Attendees discussed in pairs what an agent would do with
/// this description and three other similarly-vague tools available — this class is that
/// scenario made runnable: flip appsettings.json's DemoMode:UseVagueToolDescriptions to true,
/// restart the server, and ask the demo client the same question again. With a richer toolset
/// behind it (e.g. once the afternoon squads have added their own tools), this is the version
/// that gets skipped over or called with garbage arguments — not because the underlying API
/// changed, but because the agent had nothing but four words to go on.
///
/// Only one of this class or <see cref="RatesAndAvailabilityTool"/> is ever registered at a
/// time (see Program.cs) — both resolve to the same tool name, get_rates_and_availability, by
/// design, so the toggle is a true apples-to-apples swap of description quality alone.
/// </summary>
[McpServerToolType]
public class RatesAndAvailabilityToolVague
{
    private readonly IRatesGateway _ratesGateway;

    public RatesAndAvailabilityToolVague(IRatesGateway ratesGateway)
    {
        _ratesGateway = ratesGateway;
    }

    // No parameter-level [Description] attributes here — deliberately. An agent calling this
    // tool has only the method/parameter names and types to go on, same as a real integration
    // with a poorly-documented internal API.
    [McpServerTool]
    [Description("Gets rate data.")]
    public async Task<RatesAvailabilityResult> GetRatesAndAvailability(
        string propertyId,
        DateOnly dateFrom,
        DateOnly dateTo,
        bool includeChannelRates = false)
    {
        var result = await _ratesGateway.GetRatesAndAvailabilityAsync(propertyId, dateFrom, dateTo, includeChannelRates);
        return RatesAvailabilityResult.FromGatewayResponse(result);
    }
}
