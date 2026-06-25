using EviivoRatesMcpServer.Models;
using ModelContextProtocol;

namespace EviivoRatesMcpServer.Services;

/// <summary>
/// Fictional, deterministic stand-in for eviivo's booking/property management API. Booking
/// references follow the same EVV-YYYY-NNNNN format used in the Day 2 afternoon Web/eCommerce
/// squad's BookingApiClient stub, for consistency across the day's materials.
/// </summary>
public class InMemoryBookingGateway : IBookingGateway
{
    private static readonly Dictionary<string, BookingDetailsResult> Bookings = new()
    {
        ["EVV-2026-00781"] = new BookingDetailsResult
        {
            BookingReference = "EVV-2026-00781",
            PropertyId = "PROP-04821",
            PropertyName = "The Riverside Inn",
            GuestName = "Sarah Whitfield",
            CheckIn = new DateOnly(2026, 7, 17),
            CheckOut = new DateOnly(2026, 7, 20),
            RoomType = "Double Room, River View",
            RatePaidPerNightGbp = 119.00m,
            Status = BookingStatus.Confirmed,
            Channel = "Direct"
        },
        ["EVV-2026-00892"] = new BookingDetailsResult
        {
            BookingReference = "EVV-2026-00892",
            PropertyId = "PROP-09142",
            PropertyName = "Harbourview Hotel",
            GuestName = "Marco Bellandi",
            CheckIn = new DateOnly(2026, 7, 18),
            CheckOut = new DateOnly(2026, 7, 19),
            RoomType = "Family Suite, Harbour View",
            RatePaidPerNightGbp = 215.00m,
            Status = BookingStatus.CheckedIn,
            Channel = "Booking.com"
        }
    };

    public Task<BookingDetailsResult> GetBookingDetailsAsync(string bookingReference)
    {
        if (!Bookings.TryGetValue(bookingReference, out var booking))
        {
            throw new McpException(
                $"No booking found with reference '{bookingReference}'. Known demo bookings: {string.Join(", ", Bookings.Keys)}.");
        }

        return Task.FromResult(booking);
    }
}
