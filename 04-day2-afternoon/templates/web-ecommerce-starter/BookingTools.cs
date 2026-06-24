using ModelContextProtocol.Server;
using System.ComponentModel;

namespace WebEcommerceMcp;

// TODO (squad): this class is the home for your tool(s). The DI wiring is done —
// _api is ready to use. Your job is the [McpServerTool] method below.
//
// Reminder from the lab: write the [Description] before you write the implementation.
// If you can't say in one sentence what this tool does and when Claude should call it,
// stop and talk it through with your squad first.
[McpServerToolType]
public class BookingTools
{
    private readonly IBookingApiClient _api;

    public BookingTools(IBookingApiClient api) => _api = api;

    // Delete this comment block and write your tool here. Your starting signature
    // (from the lab README) is:
    //
    //     GetGuestConversation(bookingRef)
    //
    // Try it against "EVV-2026-00123" (a simple late-checkout request) and
    // "EVV-2026-00456" (an unresolved complaint) — they exercise different scenarios.
    // Any other booking ref returns a generic fallback thread.
    //
    // Example shape to follow — uncomment and adapt, don't just fill in blindly:
    //
    // [McpServerTool]
    // [Description("One sentence: what this tool does. One sentence: when Claude should " +
    //     "call it. One sentence: any important constraints or caveats.")]
    // public async Task<GuestConversation> GetGuestConversation(
    //     [Description("The eviivo booking reference, e.g. EVV-2026-00123")]
    //     string bookingRef)
    // {
    //     return await _api.GetGuestConversationAsync(bookingRef);
    // }
}
