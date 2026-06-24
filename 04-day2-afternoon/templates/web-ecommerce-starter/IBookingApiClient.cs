namespace WebEcommerceMcp;

/// <summary>
/// Stub of eviivo's unified guest inbox (the API behind the Guest Manager — email,
/// WhatsApp, SMS, and OTA messaging in one thread). Swap this implementation for a real
/// HTTP client once you're pointing at an actual environment — the interface is the
/// contract your MCP tool depends on, so the tool method shouldn't need to change.
/// </summary>
public interface IBookingApiClient
{
    Task<GuestConversation> GetGuestConversationAsync(string bookingRef);
}

public record GuestMessage(
    DateTime TimestampUtc,
    string Sender,
    string Text
);

public record GuestConversation(
    string BookingRef,
    string GuestName,
    string Channel,
    IReadOnlyList<GuestMessage> Messages
);
