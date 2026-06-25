namespace EviivoRatesMcpServer.Services;

/// <summary>
/// One night's raw rate/availability data as it would come back from eviivo's real
/// Rates/Availability gateway — before it's translated into the agent-facing
/// RatesAvailabilityResult / NightlyRate shape (see Models/RatesAvailabilityResult.cs).
/// </summary>
public record RatesGatewayNightlyEntry(
    DateOnly Date,
    decimal BaseRateGbp,
    int RoomsAvailable,
    int TotalRoomsCapacity,
    bool HasStopSellActive,
    IReadOnlyDictionary<string, decimal>? ChannelRates);

/// <summary>Raw gateway response for a property/date-range query.</summary>
public record RatesGatewayResponse(
    string PropertyId,
    IReadOnlyList<RatesGatewayNightlyEntry> Nights);

/// <summary>
/// Stands in for eviivo's existing internal Rates/Availability gateway service. This is the
/// "no rewrite needed" point from the Day 2 morning slides: in production you'd register your
/// real IRatesGateway implementation in Program.cs and nothing in Tools/ would need to change —
/// the MCP tool is a thin annotation layer over this interface, not a replacement for it.
/// </summary>
public interface IRatesGateway
{
    Task<RatesGatewayResponse> GetRatesAndAvailabilityAsync(
        string propertyId, DateOnly dateFrom, DateOnly dateTo, bool includeChannelRates);
}
