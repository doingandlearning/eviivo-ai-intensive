namespace WebEcommerceMcp;

/// <summary>
/// Fake-data stub so the lab doesn't depend on real eviivo API access or credentials.
/// Two booking references return hand-written, realistic-looking threads (one a simple
/// late-checkout request, one with an unresolved complaint — useful if your tool needs
/// to demonstrate handling an "escalation-worthy" conversation). Any other booking
/// reference still returns a coherent fallback thread instead of an error, so you can
/// test with arbitrary input.
/// </summary>
public class BookingApiClient : IBookingApiClient
{
    public Task<GuestConversation> GetGuestConversationAsync(string bookingRef)
    {
        var now = DateTime.UtcNow;

        if (string.Equals(bookingRef, "EVV-2026-00123", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new GuestConversation(
                BookingRef: "EVV-2026-00123",
                GuestName: "Sarah Whitfield",
                Channel: "Airbnb",
                Messages: new List<GuestMessage>
                {
                    new(now.AddHours(-5), "Guest", "Hi! We're arriving on the 2pm train — any chance of a late checkout on our last day instead, rather than early arrival?"),
                    new(now.AddHours(-4).AddMinutes(-40), "Host", "Hi Sarah, thanks for the heads up. Late checkout until 1pm on departure day works — I'll note it on the booking."),
                    new(now.AddHours(-4).AddMinutes(-35), "Guest", "That's perfect, thank you so much!")
                }));
        }

        if (string.Equals(bookingRef, "EVV-2026-00456", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new GuestConversation(
                BookingRef: "EVV-2026-00456",
                GuestName: "Marco Bellandi",
                Channel: "Booking.com",
                Messages: new List<GuestMessage>
                {
                    new(now.AddHours(-26), "Guest", "The room was not as described — there was no sea view as shown in the photos, we are very disappointed."),
                    new(now.AddHours(-25).AddMinutes(-10), "Host", "I'm sorry to hear that, Marco. Could you let me know your room number so I can look into this straight away?"),
                    new(now.AddHours(-25), "Guest", "Room 214. We'd like a partial refund or to be moved if possible."),
                    new(now.AddHours(-3), "Guest", "Following up — we haven't heard back and we check out tomorrow.")
                }));
        }

        // Fallback: still realistic, just generic, so arbitrary booking refs don't error out.
        return Task.FromResult(new GuestConversation(
            BookingRef: bookingRef,
            GuestName: "Guest",
            Channel: "Email",
            Messages: new List<GuestMessage>
            {
                new(now.AddHours(-2), "Guest", "Hi, just confirming our check-in time for this booking — is 3pm still fine?"),
                new(now.AddHours(-1).AddMinutes(-45), "Host", "Yes, 3pm works well. Looking forward to having you!")
            }));
    }
}
