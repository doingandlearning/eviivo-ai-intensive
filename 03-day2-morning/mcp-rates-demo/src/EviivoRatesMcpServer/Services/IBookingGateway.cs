using EviivoRatesMcpServer.Models;

namespace EviivoRatesMcpServer.Services;

/// <summary>
/// Stands in for eviivo's existing booking gateway / property management service.
/// Same role as IRatesGateway: a real implementation slots in here without the
/// BookingLookupTool needing to change.
/// </summary>
public interface IBookingGateway
{
    Task<BookingDetailsResult> GetBookingDetailsAsync(string bookingReference);
}
