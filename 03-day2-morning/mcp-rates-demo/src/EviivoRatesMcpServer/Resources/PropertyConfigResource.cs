using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace EviivoRatesMcpServer.Resources;

/// <summary>
/// Exposes static property configuration as an MCP Resource — the kind of context that
/// belongs in the agent's window before it starts reasoning, not returned on every tool call.
///
/// Two demo properties match the InMemoryRatesGateway so queries against the resource and
/// the tool are consistent in a live demo. In production this would call the real property
/// configuration service.
/// </summary>
[McpServerResourceType]
public class PropertyConfigResource
{
    private static readonly Dictionary<string, object> Configs = new()
    {
        ["PROP-04821"] = new
        {
            propertyId = "PROP-04821",
            name = "The Riverside Inn",
            timezone = "Europe/London",
            currency = "GBP",
            totalRooms = 10,
            yieldTarget = 0.80,
            minStayRules = new[]
            {
                new { dayOfWeek = "Friday", minNights = 2 },
                new { dayOfWeek = "Saturday", minNights = 2 }
            },
            stopSellPolicy = "Stop-sell activates automatically when fewer than 2 rooms remain on a weekend night."
        },
        ["PROP-09142"] = new
        {
            propertyId = "PROP-09142",
            name = "Harbourview Hotel",
            timezone = "Europe/London",
            currency = "GBP",
            totalRooms = 24,
            yieldTarget = 0.75,
            minStayRules = new[]
            {
                new { dayOfWeek = "Saturday", minNights = 3 }
            },
            stopSellPolicy = "Stop-sell activates automatically when fewer than 4 rooms remain on a weekend night."
        }
    };

    [McpServerResource(
        UriTemplate = "eviivo://property/{propertyId}/config",
        Name = "Property configuration",
        MimeType = "application/json")]
    [Description("Static configuration for a property: name, timezone, currency, total rooms, yield target, " +
                 "min-stay rules, and stop-sell policy. Load this before any pricing or availability task " +
                 "so you have the business rules needed to interpret rate and availability data. " +
                 "Known demo properties: PROP-04821 (The Riverside Inn), PROP-09142 (Harbourview Hotel).")]
    public static string GetPropertyConfig(string propertyId)
    {
        if (!Configs.TryGetValue(propertyId, out var config))
        {
            return $$"""
                {
                  "error": "Unknown property '{{propertyId}}'.",
                  "knownProperties": ["PROP-04821", "PROP-09142"]
                }
                """;
        }

        return System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}
