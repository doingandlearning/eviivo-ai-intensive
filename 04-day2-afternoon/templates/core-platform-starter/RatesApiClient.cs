namespace CorePlatformMcp;

/// <summary>
/// Fake-data stub so the lab doesn't depend on real eviivo API access or credentials.
/// Generates one entry per night in the requested range with deliberately realistic
/// variation: weekend uplift on price, tightening availability towards the weekend, and
/// an occasional stop-sell night so your tool has something interesting to surface.
/// </summary>
public class RatesApiClient : IRatesApiClient
{
    private const decimal WeekdayBaseRateGbp = 89.00m;
    private const decimal WeekendBaseRateGbp = 129.00m;

    public Task<IReadOnlyList<RateNight>> GetRateAvailabilityAsync(
        string propertyId, DateOnly checkIn, DateOnly checkOut)
    {
        if (checkOut <= checkIn)
        {
            throw new ArgumentException("checkOut must be after checkIn.", nameof(checkOut));
        }

        var nights = new List<RateNight>();

        for (var date = checkIn; date < checkOut; date = date.AddDays(1))
        {
            var isWeekend = date.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday;
            var baseRate = isWeekend ? WeekendBaseRateGbp : WeekdayBaseRateGbp;

            // Deterministic per-date "randomness" so the same query always returns the
            // same answer in a demo, without needing a real seeded RNG dependency.
            var dayHash = date.DayNumber % 7;

            var roomsAvailable = isWeekend ? Math.Max(0, 2 - (dayHash % 3)) : 6 - dayHash % 4;
            var hasStopSellActive = isWeekend && dayHash == 5; // occasional weekend stop-sell

            nights.Add(new RateNight(
                Date: date,
                BaseRateGbp: baseRate,
                RoomsAvailable: hasStopSellActive ? 0 : roomsAvailable,
                HasStopSellActive: hasStopSellActive));
        }

        return Task.FromResult<IReadOnlyList<RateNight>>(nights);
    }
}
