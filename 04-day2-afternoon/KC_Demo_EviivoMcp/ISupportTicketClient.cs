namespace EviivoMcp;

public interface ISupportTicketClient
{
    /// <summary>Creates a support ticket in the internal ticketing system.</summary>
    Task<SupportTicketResult> CreateTicket(CreateTicketRequest request);
}
