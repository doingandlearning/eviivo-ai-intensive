namespace EviivoMcp;

public record BookingStatus(
    string BookingRef,
    string GuestName,
    string PropertyName,
    DateOnly CheckIn,
    DateOnly CheckOut,
    string Status,
    bool IsValid
);
