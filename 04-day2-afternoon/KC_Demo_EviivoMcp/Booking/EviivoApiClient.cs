namespace EviivoMcp;

public class EviivoApiClient : IEviivoApiClient
{
    public Task<BookingStatus> GetBookingStatus(string bookingRef)
    {
        var result = bookingRef.Trim().ToLowerInvariant() switch
        {
            "evv-2026-00123" => new BookingStatus(
                BookingRef: bookingRef,
                GuestName: "Jane Smith",
                PropertyName: "The Grand Hotel",
                CheckIn: new DateOnly(2026, 06, 21),
                CheckOut: new DateOnly(2026, 06, 27),
                Status: "confirmed",
                IsValid: true),

            _ => new BookingStatus(
                BookingRef: bookingRef,
                GuestName: "Unknown",
                PropertyName: "Unknown",
                CheckIn: DateOnly.FromDateTime(DateTime.UtcNow),
                CheckOut: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                Status: "not_found",
                IsValid: false)
        };

        return Task.FromResult(result);
    }
}
