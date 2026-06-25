using System.ComponentModel;
using EviivoRatesMcpServer.Services;

namespace EviivoRatesMcpServer.Models;

/// <summary>
/// One night's rate and availability state. Field-level [Description] attributes are read
/// by the agent, not just by developers — this is "schema as agent documentation" (see the
/// Day 2 morning slide "What the output schema looks like — and why it matters").
/// </summary>
public record NightlyRate
{
    [Description("The calendar date this entry covers, ISO 8601 format.")]
    public required DateOnly Date { get; init; }

    [Description("The base rate for this night in GBP, before any channel-specific adjustments.")]
    public required decimal BaseRateGbp { get; init; }

    [Description("Number of rooms available to sell on this night, after stop-sell restrictions " +
                 "are applied. This will be 0 on a stop-sell night even if raw inventory says otherwise.")]
    public required int RoomsAvailable { get; init; }

    [Description("True if a stop-sell restriction is active on this specific night.")]
    public bool HasStopSellActive { get; init; }

    [Description("Rate broken down by OTA channel (e.g. \"Airbnb\", \"Booking.com\", \"Direct\"), in GBP. " +
                 "Only populated when the request set include_channel_rates=true; otherwise null.")]
    public IReadOnlyDictionary<string, decimal>? ChannelRates { get; init; }
}

/// <summary>
/// Structured result for get_rates_and_availability. This is the exact shape from the Day 2
/// morning slide "What the output schema looks like — and why it matters" — Nights,
/// HasStopSellActive, LowAvailabilityNightCount are all named and described to match.
/// </summary>
public record RatesAvailabilityResult
{
    [Description("Rates and availability by date. Each entry covers one night.")]
    public required IReadOnlyList<NightlyRate> Nights { get; init; }

    [Description("True if any night in the range has a stop-sell restriction active.")]
    public bool HasStopSellActive { get; init; }

    [Description("Count of nights where available rooms drops below 20% of capacity.")]
    public int LowAvailabilityNightCount { get; init; }

    /// <summary>
    /// Maps the internal gateway response shape onto the agent-facing structured result.
    /// This split mirrors the slide's framing: _ratesGateway is your existing service —
    /// this method is the thin, agent-facing translation layer on top of it.
    /// </summary>
    public static RatesAvailabilityResult FromGatewayResponse(RatesGatewayResponse response)
    {
        var nights = response.Nights
            .Select(n => new NightlyRate
            {
                Date = n.Date,
                BaseRateGbp = n.BaseRateGbp,
                RoomsAvailable = n.RoomsAvailable,
                HasStopSellActive = n.HasStopSellActive,
                ChannelRates = n.ChannelRates
            })
            .ToList();

        var lowAvailabilityNightCount = response.Nights.Count(n =>
            n.TotalRoomsCapacity > 0 && (double)n.RoomsAvailable / n.TotalRoomsCapacity < 0.2);

        return new RatesAvailabilityResult
        {
            Nights = nights,
            HasStopSellActive = nights.Any(n => n.HasStopSellActive),
            LowAvailabilityNightCount = lowAvailabilityNightCount
        };
    }
}
