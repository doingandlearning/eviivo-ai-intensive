namespace EviivoMcp;

/// <summary>Request model for creating a support ticket.</summary>
public record CreateTicketRequest(
    string BookingRef,
    string GuestName,
    string Subject,
    string Description,
    string Priority   // "low" | "medium" | "high" | "urgent"
);

/// <summary>Result returned after a support ticket is created.</summary>
public record SupportTicketResult(
    string TicketId,
    string Status,     // "created" | "duplicate" | "error"
    string Message
);
