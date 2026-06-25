using EviivoRatesMcpServer.Prompts;
using EviivoRatesMcpServer.Resources;
using EviivoRatesMcpServer.Services;
using EviivoRatesMcpServer.Tools;

var builder = WebApplication.CreateBuilder(args);

// Stand-ins for eviivo's existing internal services. In production these registrations would
// point at the real Rates/Availability and booking gateways — nothing in Tools/ would change.
builder.Services.AddSingleton<IRatesGateway, InMemoryRatesGateway>();
builder.Services.AddSingleton<IBookingGateway, InMemoryBookingGateway>();

// The toggle behind the demo's "what breaks with a vague tool description" segment. See
// appsettings.json and README.md, "Demo script — part 2: the failure mode."
var useVagueToolDescriptions = builder.Configuration.GetValue<bool>("DemoMode:UseVagueToolDescriptions");

var mcpServerBuilder = builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        // Stateless is the SDK's recommended default for servers that don't need
        // server-to-client requests (sampling/elicitation) — see
        // https://csharp.sdk.modelcontextprotocol.io/concepts/stateless/stateless.html.
        // It also means this demo needs no session/affinity handling, which keeps the
        // "restart the server to flip the toggle" demo step simple.
        options.Stateless = true;
    });

// Exactly one of these two is registered at a time. Both classes expose a method named
// GetRatesAndAvailability, so both resolve to the same tool name (get_rates_and_availability) —
// registering both simultaneously would be a duplicate-tool-name conflict, not a fair
// comparison. Flip DemoMode:UseVagueToolDescriptions in appsettings.json and restart to swap.
if (useVagueToolDescriptions)
{
    mcpServerBuilder.WithTools<RatesAndAvailabilityToolVague>();
}
else
{
    mcpServerBuilder.WithTools<RatesAndAvailabilityTool>();
}

// Always registered, regardless of the toggle above — this is the contrasting tool that gives
// list_tools() a genuine second option to choose between.
mcpServerBuilder.WithTools<BookingLookupTool>();

// Resource: static property configuration (yield target, min-stay rules, stop-sell policy).
// Demonstrates the Resources primitive — context the agent loads before acting rather than
// data returned from a tool call. URI template: eviivo://property/{propertyId}/config
// mcpServerBuilder.WithResources<PropertyConfigResource>();

// Prompt: structured rate review template. Demonstrates the Prompts primitive — a reusable
// server-side task template the agent receives and executes step by step.
// mcpServerBuilder.WithPrompts<RateReviewPrompt>();

var app = builder.Build();

// Maps the Streamable HTTP endpoint at the root path. The DemoClient project (and MCP
// Inspector / Claude Desktop / Claude Code, per README.md) connect here.
app.MapMcp();

app.Run();
