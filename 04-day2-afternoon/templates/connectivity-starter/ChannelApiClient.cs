namespace ConnectivityMcp;

/// <summary>
/// Fake-data stub so the lab doesn't depend on real eviivo API access or credentials.
/// Returns a different, deliberately realistic scenario per channel so you can see your
/// tool handle a healthy sync, a stalled sync, and a hard error without changing any inputs
/// other than the channel name.
/// </summary>
public class ChannelApiClient : IChannelApiClient
{
    public Task<ChannelSyncStatus> GetChannelSyncStatusAsync(string propertyId, string channel)
    {
        var now = DateTime.UtcNow;

        ChannelSyncStatus result = channel.Trim().ToLowerInvariant() switch
        {
            "airbnb" => new ChannelSyncStatus(
                PropertyId: propertyId,
                Channel: "Airbnb",
                LastSyncedAtUtc: now.AddMinutes(-4),
                Status: SyncStatus.Synced,
                PendingUpdateCount: 0,
                LastError: null),

            "booking.com" => new ChannelSyncStatus(
                PropertyId: propertyId,
                Channel: "Booking.com",
                LastSyncedAtUtc: now.AddHours(-3),
                Status: SyncStatus.Error,
                PendingUpdateCount: 2,
                LastError: "Rate plan mapping rejected: missing cancellation policy for non-refundable plan 'NR-STD'"),

            "expedia" => new ChannelSyncStatus(
                PropertyId: propertyId,
                Channel: "Expedia",
                LastSyncedAtUtc: now.AddMinutes(-47),
                Status: SyncStatus.PendingUpdates,
                PendingUpdateCount: 3,
                LastError: null),

            "vrbo" => new ChannelSyncStatus(
                PropertyId: propertyId,
                Channel: "Vrbo",
                LastSyncedAtUtc: now.AddMinutes(-12),
                Status: SyncStatus.Synced,
                PendingUpdateCount: 0,
                LastError: null),

            _ => new ChannelSyncStatus(
                PropertyId: propertyId,
                Channel: channel,
                LastSyncedAtUtc: now.AddHours(-1),
                Status: SyncStatus.PendingUpdates,
                PendingUpdateCount: 1,
                LastError: null)
        };

        return Task.FromResult(result);
    }
}
