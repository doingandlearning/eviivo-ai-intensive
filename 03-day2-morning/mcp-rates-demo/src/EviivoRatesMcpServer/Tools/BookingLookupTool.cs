using System.ComponentModel;
using EviivoRatesMcpServer.Models;
using EviivoRatesMcpServer.Services;
using ModelContextProtocol.Server;

namespace EviivoRatesMcpServer.Tools;

/// <summary>
/// A second, contrasting tool — always registered regardless of the vague/good toggle on
/// get_rates_and_availability (see Program.cs). It exists for two reasons:
///
/// 1. It answers a genuinely different question (one reservation's details, not a date-range
///    rate shop), so list_tools() in the demo always shows at least two real choices — without
///    it, "call the only tool available" wouldn't prove anything about description quality.
/// 2. Its description explicitly points back at get_rates_and_availability, modelling the kind
///    of disambiguating language the morning session recommends when a toolset has more than
///    one tool that could plausibly apply to a request.
///
/// Booking references follow the same EVV-YYYY-NNNNN format used by the Day 2 afternoon
/// Web/eCommerce squad's BookingApiClient stub.
/// </summary>
[McpServerToolType]
public class BookingLookupTool
{
    private readonly IBookingGateway _bookingGateway;

    public BookingLookupTool(IBookingGateway bookingGateway)
    {
        _bookingGateway = bookingGateway;
    }

    [McpServerTool]
    [Description("Looks up an existing reservation by its booking reference and returns guest, stay, " +
                 "and status details. Use this when you have a specific booking reference and need " +
                 "details about that one reservation — not for checking rates or availability across " +
                 "a date range; for that, use get_rates_and_availability instead.")]
    public async Task<BookingDetailsResult> GetBookingDetails(
        [Description("The eviivo booking reference to look up, e.g. \"EVV-2026-00781\".")] string bookingReference)
    {
        return await _bookingGateway.GetBookingDetailsAsync(bookingReference);
    }
}
