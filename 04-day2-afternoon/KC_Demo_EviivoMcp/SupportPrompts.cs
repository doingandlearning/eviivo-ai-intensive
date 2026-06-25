using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerPromptType]
public class SupportPrompts
{
    [McpServerPrompt(Name = "guest_support_orchestrator"), Description(
        "Orchestrates a full guest support response: analyses sentiment, looks up the booking, " +
        "creates a support ticket if needed, and drafts a reply to the guest."
    )]
    public static IEnumerable<ChatMessage> GuestSupportOrchestrator(
        [Description("The raw text of the guest's message or complaint.")]
        string guestMessage,

        [Description("The booking reference supplied by the guest.")]
        string bookingRef
    )
    {
        var prompt = $"""
            You are a guest support agent for Eviivo, a hospitality property management platform.
            A guest has contacted support. Handle their request end-to-end by following these steps in order:

            ## Step 1 — Analyse Sentiment
            Call the `AnalyseSentiment` tool with the guest message below.
            Note the `sentiment`, `score`, and `urgency` returned.
            If sentiment is **positive** and urgency is **low**, thank the guest and close the interaction — no ticket needed.

            ## Step 2 — Look Up the Booking
            Call `GetBookingStatus` with the booking reference below.
            - If `IsValid` is false, tell the guest the reference could not be found and ask them to verify it.
            - If `IsValid` is true, continue to Step 3.

            ## Step 3 — Create a Support Ticket
            Call `CreateSupportTicket` using:
            - `bookingRef`: the reference below
            - `guestName`: the GuestName returned by GetBookingStatus
            - `subject`: a concise one-line summary you write
            - `description`: the guest's original message plus any relevant booking details
            - `priority`: map urgency → priority (high → "urgent", medium → "high", low → "medium")

            ## Step 4 — Reply to the Guest
            Write a professional, empathetic reply that includes:
            1. Acknowledgement of their concern
            2. A summary of the issue as you understood it
            3. The ticket ID so they can track progress
            4. Resolution timeframe: urgent → 2 hours, high → 4 hours, medium → 1 business day

            ---
            **Guest message:** {guestMessage}
            **Booking reference:** {bookingRef}
            """;

        yield return new ChatMessage(ChatRole.User, prompt);
    }
}
