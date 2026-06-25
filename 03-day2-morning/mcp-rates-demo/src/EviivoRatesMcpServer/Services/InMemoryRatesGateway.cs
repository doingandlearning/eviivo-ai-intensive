using ModelContextProtocol;

namespace EviivoRatesMcpServer.Services;

/// <summary>
/// Fictional, deterministic stand-in for eviivo's real Rates/Availability gateway so the demo
/// doesn't depend on live API access or credentials. Two demo properties, weekday/weekend
/// pricing, and an occasional weekend stop-sell night — same texture as the Day 2 afternoon
/// Core Platform squad's RatesApiClient stub, so the two halves of the day feel like one
/// continuous world rather than two unrelated examples.
/// </summary>
public class InMemoryRatesGateway : IRatesGateway
{
    private sealed record PropertyProfile(string Name, decimal WeekdayRateGbp, decimal WeekendRateGbp, int TotalRooms);

    private static readonly Dictionary<string, PropertyProfile> Properties = new()
    {
        ["PROP-04821"] = new PropertyProfile("The Riverside Inn", WeekdayRateGbp: 89.00m, WeekendRateGbp: 129.00m, TotalRooms: 10),
        ["PROP-09142"] = new PropertyProfile("Harbourview Hotel", WeekdayRateGbp: 142.00m, WeekendRateGbp: 215.00m, TotalRooms: 24),
    };

    public Task<RatesGatewayResponse> GetRatesAndAvailabilityAsync(
        string propertyId, DateOnly dateFrom, DateOnly dateTo, bool includeChannelRates)
    {
        // McpException's message is what gets surfaced back to the calling agent as a
        // tool-level error (as opposed to a generic protocol failure) — see
        // https://csharp.sdk.modelcontextprotocol.io for the distinction.
        if (!Properties.TryGetValue(propertyId, out var profile))
        {
            throw new McpException(
                $"Unknown property_id '{propertyId}'. Known demo properties: {string.Join(", ", Properties.Keys)}.");
        }

        if (dateTo < dateFrom)
        {
            throw new McpException("date_to must be on or after date_from.");
        }

        var nights = new List<RatesGatewayNightlyEntry>();

        // date_to is INCLUSIVE per the tool description below — note the <= here, not <.
        // (Keeping the loop's actual behaviour matched to what the description promises is
        // exactly the discipline the morning session is making the case for.)
        for (var date = dateFrom; date <= dateTo; date = date.AddDays(1))
        {
            var isWeekend = date.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday;
            var baseRate = isWeekend ? profile.WeekendRateGbp : profile.WeekdayRateGbp;

            // Deterministic per-date variation so the same query always returns the same
            // answer in a live demo, without needing a seeded RNG dependency.
            var dayHash = date.DayNumber % 7;

            var roomsAvailable = isWeekend
                ? Math.Max(0, profile.TotalRooms / 5 - dayHash % 3)
                : Math.Max(0, profile.TotalRooms / 2 - dayHash % 4);

            // Occasional weekend stop-sell, same texture as the afternoon's RatesApiClient stub.
            var hasStopSellActive = isWeekend && dayHash == 5;

            IReadOnlyDictionary<string, decimal>? channelRates = null;
            if (includeChannelRates)
            {
                channelRates = new Dictionary<string, decimal>
                {
                    ["Airbnb"] = Math.Round(baseRate * 1.05m, 2),
                    ["Booking.com"] = Math.Round(baseRate * 1.08m, 2),
                    ["Direct"] = baseRate
                };
            }

            nights.Add(new RatesGatewayNightlyEntry(
                Date: date,
                BaseRateGbp: baseRate,
                RoomsAvailable: hasStopSellActive ? 0 : roomsAvailable,
                TotalRoomsCapacity: profile.TotalRooms,
                HasStopSellActive: hasStopSellActive,
                ChannelRates: channelRates));
        }

        return Task.FromResult(new RatesGatewayResponse(propertyId, nights));
    }
}
