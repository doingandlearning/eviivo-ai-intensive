namespace EviivoMcp;

/// <summary>
/// Stub implementation of ISupportTicketClient.
/// Replace the body of CreateTicket with a real HTTP call to your internal
/// ticketing API (e.g. Zendesk, Freshdesk, or a bespoke Eviivo endpoint).
/// </summary>
public class SupportTicketClient : ISupportTicketClient
{
    private static int _counter = 1000;

    public Task<SupportTicketResult> CreateTicket(CreateTicketRequest request)
    {
        // --- Stub logic: always succeeds and mints a sequential ticket ID ---
        var id = $"TKT-{Interlocked.Increment(ref _counter)}";

        var result = new SupportTicketResult(
            TicketId: id,
            Status: "created",
            Message: $"Support ticket {id} created successfully for booking {request.BookingRef}."
        );

        return Task.FromResult(result);
    }
}
