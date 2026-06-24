using ModelContextProtocol.Server;
using System.ComponentModel;

namespace ConnectivityMcp;

// TODO (squad): this class is the home for your tool(s). The DI wiring is done —
// _api is ready to use. Your job is the [McpServerTool] method below.
//
// Reminder from the lab: write the [Description] before you write the implementation.
// If you can't say in one sentence what this tool does and when Claude should call it,
// stop and talk it through with your squad first.
[McpServerToolType]
public class ConnectivityTools
{
    private readonly IChannelApiClient _api;

    public ConnectivityTools(IChannelApiClient api) => _api = api;

    // Delete this comment block and write your tool here. Your starting signature
    // (from the lab README) is:
    //
    //     GetChannelSyncStatus(propertyId, channel)
    //
    // Example shape to follow — uncomment and adapt, don't just fill in blindly:
    //
    // [McpServerTool]
    // [Description("One sentence: what this tool does. One sentence: when Claude should " +
    //     "call it. One sentence: any important constraints or caveats.")]
    // public async Task<ChannelSyncStatus> GetChannelSyncStatus(
    //     [Description("The eviivo property ID, e.g. PROP-04821")]
    //     string propertyId,
    //     [Description("The OTA channel name, e.g. Airbnb, Booking.com, Expedia, Vrbo")]
    //     string channel)
    // {
    //     return await _api.GetChannelSyncStatusAsync(propertyId, channel);
    // }
}
