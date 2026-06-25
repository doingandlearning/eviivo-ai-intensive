// Scripted MCP client for the Day 2 morning demo. Walks through the brief's four steps against
// a running EviivoRatesMcpServer instance:
//   1. list_tools()      — the agent discovers what's available.
//   2. call_tool(...)     — a typed call with property ID and date range.
//   3. structured response — what the agent actually gets back.
//   4. composed recommendation — what the agent does with it.
//
// This client always calls get_rates_and_availability by name, so it does NOT exercise the
// "vague vs good description" failure mode itself — that failure mode is about an LLM *choosing*
// which tool to call from its description, and a hardcoded call bypasses that choice entirely.
// To see the failure mode live, point an actual LLM-backed MCP client (Claude Desktop, Claude
// Code, or MCP Inspector) at the server instead, after flipping DemoMode:UseVagueToolDescriptions
// in the Server's appsettings.json. See README.md, "Demo script — part 2."
using System.Text.Json;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

const string ServerEndpoint = "http://localhost:5179";

Console.WriteLine("=== eviivo Rates & Availability MCP demo client ===");
Console.WriteLine($"Connecting to {ServerEndpoint} ...");
Console.WriteLine();

McpClient client;
try
{
    var transport = new HttpClientTransport(new HttpClientTransportOptions
    {
        Endpoint = new Uri(ServerEndpoint),
        TransportMode = HttpTransportMode.StreamableHttp,
    });

    client = await McpClient.CreateAsync(transport);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Couldn't connect to {ServerEndpoint}: {ex.Message}");
    Console.Error.WriteLine("Is the server running? From src/EviivoRatesMcpServer: dotnet run");
    return;
}

await using (client)
{
    // -----------------------------------------------------------------------
    // Step 1 — list_tools(): the agent discovers what's available.
    // -----------------------------------------------------------------------
    Console.WriteLine("Step 1: list_tools()");
    Console.WriteLine(new string('-', 60));

    var tools = await client.ListToolsAsync();
    foreach (var tool in tools)
    {
        Console.WriteLine($"  - {tool.Name}: {tool.Description}");
    }
    Console.WriteLine();

    var ratesTool = tools.FirstOrDefault(t => t.Name == "get_rates_and_availability");
    if (ratesTool is null)
    {
        Console.Error.WriteLine("get_rates_and_availability was not in the tool list — check the server is on the expected build.");
        return;
    }

    // -----------------------------------------------------------------------
    // Step 2 — call_tool("get_rates_and_availability", { property ID, date range }).
    // -----------------------------------------------------------------------
    Console.WriteLine("Step 2: call_tool(\"get_rates_and_availability\", { ... })");
    Console.WriteLine(new string('-', 60));

    const string propertyId = "PROP-04821"; // The Riverside Inn — see Services/InMemoryRatesGateway.cs
    var dateFrom = new DateOnly(2026, 7, 17);
    var dateTo = new DateOnly(2026, 7, 20);

    var arguments = new Dictionary<string, object?>
    {
        ["propertyId"] = propertyId,
        ["dateFrom"] = dateFrom.ToString("yyyy-MM-dd"),
        ["dateTo"] = dateTo.ToString("yyyy-MM-dd"),
        ["includeChannelRates"] = false
    };

    Console.WriteLine($"  propertyId:          {propertyId}");
    Console.WriteLine($"  dateFrom:            {dateFrom:yyyy-MM-dd}");
    Console.WriteLine($"  dateTo:              {dateTo:yyyy-MM-dd}");
    Console.WriteLine($"  includeChannelRates: false");
    Console.WriteLine();

    CallToolResult result = await client.CallToolAsync(ratesTool.Name, arguments);

    if (result.IsError == true)
    {
        var errorText = result.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text ?? "(no error detail returned)";
        Console.Error.WriteLine($"Tool call failed: {errorText}");
        return;
    }

    // -----------------------------------------------------------------------
    // Step 3 — the structured response the agent receives.
    // -----------------------------------------------------------------------
    Console.WriteLine("Step 3: structured response (CallToolResult.StructuredContent)");
    Console.WriteLine(new string('-', 60));

    if (result.StructuredContent is not { } structuredContent)
    {
        Console.Error.WriteLine("Tool call succeeded but returned no structured content.");
        return;
    }

    Console.WriteLine(JsonSerializer.Serialize(structuredContent, new JsonSerializerOptions { WriteIndented = true }));
    Console.WriteLine();

    // -----------------------------------------------------------------------
    // Step 4 — the agent composing a recommendation from that response.
    // -----------------------------------------------------------------------
    Console.WriteLine("Step 4: composed recommendation");
    Console.WriteLine(new string('-', 60));

    try
    {
        Console.WriteLine(ComposeRecommendation(propertyId, structuredContent));
    }
    catch (Exception ex)
    {
        // Defensive: if RatesAvailabilityResult's shape ever changes without this method being
        // updated to match, fail loudly here instead of taking down the rest of a live demo.
        Console.Error.WriteLine($"Could not compose a recommendation from the structured response: {ex.Message}");
    }

    Console.WriteLine();
    Console.WriteLine(new string('=', 60));
    Console.WriteLine("To see the 'vague description' failure mode: set");
    Console.WriteLine("DemoMode:UseVagueToolDescriptions to true in the Server's");
    Console.WriteLine("appsettings.json, restart the server, then connect an actual");
    Console.WriteLine("LLM-backed MCP client (Claude Desktop, Claude Code, or MCP");
    Console.WriteLine("Inspector) rather than this scripted client. See README.md,");
    Console.WriteLine("'Demo script — part 2: the failure mode.'");
}

// Stands in for the reasoning an LLM agent would do directly from the structured content above.
// Written as plain code (no LLM call) so the demo runs standalone with no API key or network
// dependency. Property names below are camelCase because the SDK's default JSON options use
// System.Text.Json's "Web" defaults (camelCase) — see McpJsonUtilities.DefaultOptions.
static string ComposeRecommendation(string propertyId, JsonElement structuredContent)
{
    var nights = structuredContent.GetProperty("nights");
    var hasStopSellActive = structuredContent.GetProperty("hasStopSellActive").GetBoolean();
    var lowAvailabilityNightCount = structuredContent.GetProperty("lowAvailabilityNightCount").GetInt32();

    var nightCount = 0;
    decimal rateTotal = 0m;
    var lowestRoomsAvailable = int.MaxValue;

    foreach (var night in nights.EnumerateArray())
    {
        nightCount++;
        rateTotal += night.GetProperty("baseRateGbp").GetDecimal();
        lowestRoomsAvailable = Math.Min(lowestRoomsAvailable, night.GetProperty("roomsAvailable").GetInt32());
    }

    if (nightCount == 0)
    {
        return $"No nights were returned for {propertyId} in this range.";
    }

    var averageRate = rateTotal / nightCount;

    var headline = hasStopSellActive
        ? $"Heads up: {propertyId} has at least one stop-sell night in this range."
        : $"{propertyId} has open availability across all {nightCount} night(s) in this range.";

    var availabilityNote = lowAvailabilityNightCount > 0
        ? $" {lowAvailabilityNightCount} night(s) are below 20% availability (lowest: {lowestRoomsAvailable} room(s) left)."
        : " Availability looks healthy throughout.";

    return $"{headline}{availabilityNote} Average base rate: £{averageRate:0.00}/night.";
}
