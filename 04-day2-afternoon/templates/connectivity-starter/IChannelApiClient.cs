namespace ConnectivityMcp;

/// <summary>
/// Stub of eviivo's internal channel management API. Swap this implementation for a real
/// HTTP client once you're pointing at an actual environment — the interface is the contract
/// your MCP tool depends on, so the tool method shouldn't need to change.
/// </summary>
public interface IChannelApiClient
{
    Task<ChannelSyncStatus> GetChannelSyncStatusAsync(string propertyId, string channel);
}

public enum SyncStatus
{
    Synced,
    PendingUpdates,
    Error
}

/// <summary>
/// Shape mirrors what the real channel management gateway returns for a single
/// property/channel pair (STP integration status against Airbnb, Booking.com, Expedia, Vrbo, etc).
/// </summary>
public record ChannelSyncStatus(
    string PropertyId,
    string Channel,
    DateTime LastSyncedAtUtc,
    SyncStatus Status,
    int PendingUpdateCount,
    string? LastError
);
