namespace CorePlatformMcp;

/// <summary>
/// Stub of eviivo's internal rates and availability gateway. Swap this implementation for
/// a real HTTP client once you're pointing at an actual environment — the interface is the
/// contract your MCP tool depends on, so the tool method shouldn't need to change.
/// </summary>
public interface IRatesApiClient
{
    Task<IReadOnlyList<RateNight>> GetRateAvailabilityAsync(
        string propertyId, DateOnly checkIn, DateOnly checkOut);
}

/// <summary>
/// One night's rate/availability state. <c>HasStopSellActive</c> mirrors the same field name
/// used in the Day 2 morning design exercise — a stop-sell night should never be sold even if
/// rooms are technically available.
/// </summary>
public record RateNight(
    DateOnly Date,
    decimal BaseRateGbp,
    int RoomsAvailable,
    bool HasStopSellActive
);
