namespace EviivoMcp;

public interface IEviivoApiClient
{
    Task<BookingStatus> GetBookingStatus(string bookingRef);
}
