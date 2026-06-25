using System.ComponentModel;
using System.Text.Json.Serialization;

namespace EviivoRatesMcpServer.Models;

/// <summary>Current status of a booking, returned by get_booking_details.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BookingStatus
{
    Confirmed,
    CheckedIn,
    CheckedOut,
    Cancelled
}

/// <summary>
/// Structured result for get_booking_details — the contrasting "booking lookup" tool from
/// the demo brief. Deliberately a different shape and a different domain question from
/// get_rates_and_availability: this is per-reservation detail, not a date-range rate shop.
/// </summary>
public record BookingDetailsResult
{
    [Description("The eviivo booking reference this result is for.")]
    public required string BookingReference { get; init; }

    [Description("The property identifier the booking is for.")]
    public required string PropertyId { get; init; }

    [Description("Human-readable property name.")]
    public required string PropertyName { get; init; }

    [Description("Name of the guest the booking is under.")]
    public required string GuestName { get; init; }

    [Description("Check-in date, ISO 8601 format.")]
    public required DateOnly CheckIn { get; init; }

    [Description("Check-out date, ISO 8601 format.")]
    public required DateOnly CheckOut { get; init; }

    [Description("Room type booked, e.g. \"Double Room, River View\".")]
    public required string RoomType { get; init; }

    [Description("Rate paid per night in GBP, inclusive of any discount applied at time of booking.")]
    public required decimal RatePaidPerNightGbp { get; init; }

    [Description("Current status of the booking: Confirmed, CheckedIn, CheckedOut, or Cancelled.")]
    public required BookingStatus Status { get; init; }

    [Description("Booking source channel, e.g. \"Direct\", \"Airbnb\", \"Booking.com\".")]
    public required string Channel { get; init; }
}
