using EviivoMcp;
using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerToolType]
public class SupportTicketTools
{
    private readonly ISupportTicketClient _tickets;

    public SupportTicketTools(ISupportTicketClient tickets) => _tickets = tickets;

    [McpServerTool, Description(
        "Creates a support ticket in the internal ticketing system on behalf of a guest. " +
        "Use this when a booking issue cannot be resolved immediately and requires " +
        "follow-up by the support team."
    )]
    public async Task<SupportTicketResult> CreateSupportTicket(
        [Description("The booking reference number associated with the complaint.")]
        string bookingRef,

        [Description("Full name of the guest raising the issue.")]
        string guestName,

        [Description("Short one-line subject summarising the issue.")]
        string subject,

        [Description("Full description of the problem, including any relevant context.")]
        string description,

        [Description("Priority level: 'low', 'medium', 'high', or 'urgent'. " +
                     "Base this on the sentiment urgency returned by AnalyseSentiment.")]
        string priority
    )
    {
        var request = new CreateTicketRequest(bookingRef, guestName, subject, description, priority);
        return await _tickets.CreateTicket(request);
    }
}
