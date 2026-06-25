using EviivoMcp;
using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerToolType]
public class BookingTools
{
	private readonly IEviivoApiClient _api;

	public BookingTools(IEviivoApiClient api) => _api = api;

	[McpServerTool, Description(
		"Returns the current status of a booking, given its reference number. " +
		"Use when a guest reports a problem and you need to check if the booking is still valid."
	)]
	public async Task<BookingStatus> GetBookingStatus(
		[Description("The booking reference number to check.")]
		string bookingRef
	)
	{
		return await _api.GetBookingStatus(bookingRef);
	}

}