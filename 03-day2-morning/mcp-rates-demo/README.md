# eviivo Rates & Availability MCP Demo

Scaffold for the Day 2 morning live demo (`day2-morning/slides.md`, "Demo" section) in the
eviivo AI Engineering Intensive. It's also meant to be a starting point for the Day 2 afternoon
build lab — see "Building on this scaffold" below.

It's an ASP.NET Core Web API exposing a Model Context Protocol (MCP) server over Streamable
HTTP, plus a small console client that walks through the demo step by step. Two demo properties,
two demo bookings, all fictional, all in-memory — no credentials, no live eviivo API access
needed to run this.

## What's here

```
mcp-rates-demo/
├── README.md                          ← this file
└── src/
    ├── EviivoRatesMcpServer/          ← the MCP server (ASP.NET Core, Streamable HTTP)
    │   ├── Tools/                     ← [McpServerTool]-annotated methods agents call
    │   ├── Models/                    ← structured result types (the "output schema")
    │   ├── Services/                  ← fictional in-memory data + gateway interfaces
    │   ├── Program.cs
    │   └── appsettings.json           ← demo mode toggle lives here
    └── EviivoRatesMcpDemoClient/      ← scripted console client for the live demo
        └── Program.cs
```

## Prerequisites

- .NET 8 SDK (same requirement as the Day 2 afternoon lab)

I wasn't able to run a .NET compiler in the environment this was built in, so **the first thing
to do with this scaffold is `dotnet build` both projects** before relying on it live. Everything
here was written and cross-checked against the official MCP C# SDK docs
(https://csharp.sdk.modelcontextprotocol.io) and a live NuGet feed, but a compiler catches things
documentation review can't.

## Quickstart

Terminal 1 — start the server:

```bash
cd src/EviivoRatesMcpServer
dotnet build      # do this first — see note above
dotnet run
```

It listens on `http://localhost:5179` (see `Properties/launchSettings.json`).

Terminal 2 — run the scripted demo client against it:

```bash
cd src/EviivoRatesMcpDemoClient
dotnet run
```

## Demo script

This maps directly onto the four beats in the Day 2 morning slides' "Demo" section.

### Part 1 — the happy path (what the DemoClient shows)

1. **`list_tools()`** — the client connects and lists what the server exposes:
   `get_rates_and_availability` and `get_booking_details`, each with its description.
2. **`call_tool("get_rates_and_availability", { propertyId, dateFrom, dateTo })`** — a typed
   call against `PROP-04821` ("The Riverside Inn") for 17–20 July 2026.
3. **The structured response** — `CallToolResult.StructuredContent`, printed as raw JSON:
   nightly rates, a `hasStopSellActive` flag, a `lowAvailabilityNightCount`.
4. **The composed recommendation** — a short human-readable summary built from that structured
   data ("open availability across all 3 nights... average base rate: £..."). In the
   `DemoClient` this is plain C# standing in for what an LLM agent would do directly from the
   structured content — written that way so the demo runs with no LLM API key or network call.
   Worth saying out loud live: a real agent does this same reasoning *itself*, from the schema —
   that's the entire pitch for typed, well-described structured output.

### Part 2 — the failure mode (vague vs. good tool descriptions)

This is a callback to the morning's own Exercise 2 — the same two descriptions attendees
already discussed in pairs, made runnable:

> **Description A:** `Gets rate data.`
>
> **Description B:** `Returns current rates and room availability for a property across a date
> range. Use this when you need to assess pricing, occupancy, or yield opportunities. Returns
> rate tiers, channel restrictions, and available room count per night.`

`Tools/RatesAndAvailabilityTool.cs` carries Description B (the default). Its twin,
`Tools/RatesAndAvailabilityToolVague.cs`, carries Description A verbatim and strips all
parameter-level `[Description]` text too — same method, same gateway underneath, only the
documentation changed. `Program.cs` registers exactly one of the two, controlled by
`appsettings.json`:

```json
"DemoMode": { "UseVagueToolDescriptions": false }
```

To run the failure mode live:

1. Set `UseVagueToolDescriptions` to `true`, restart the server.
2. Connect an **actual LLM-backed MCP client** — Claude Desktop, Claude Code, or MCP Inspector —
   not the scripted `DemoClient`. The `DemoClient` calls the tool by name on purpose, which
   sidesteps the thing actually being demonstrated: an agent *choosing* which tool to call from
   nothing but its description. That choice only becomes visible (and breaks) once a model is
   the one reading the tool list.
3. Ask the same kind of question the morning's Exercise 2 poses — "are we overpriced for next
   weekend compared to last year?" — and watch what the agent does with a toolset where one tool
   says `Gets rate data.` This is also where it's worth pointing at `get_booking_details`
   alongside it: with two vaguely-described tools in the list instead of one, the agent has even
   less to go on when deciding between them.

## Connecting other MCP clients

The server speaks Streamable HTTP at the root path (`http://localhost:5179`), stateless mode.

- **MCP Inspector:** `npx @modelcontextprotocol/inspector`, then connect to
  `http://localhost:5179` with transport "Streamable HTTP".
- **Claude Desktop / Claude Code:** add an HTTP MCP server pointing at
  `http://localhost:5179`. Consult current Claude Code / Claude Desktop docs for the exact
  config syntax, since this changes between releases.

## Why HTTP here, but stdio in the afternoon lab

This is a deliberate, not accidental, difference from the Day 2 afternoon squad starters (which
all use `WithStdioServerTransport()`):

- **stdio** suits the afternoon's pattern — each squad's MCP server runs as a local child
  process of one editor/IDE integration on one laptop. No networking, no deployment, minimal
  ceremony.
- **Streamable HTTP** (stateless) suits this demo because it's instructor-led and projected: it
  needs to be reachable as a normal local web service, restartable mid-session to flip the
  description toggle above, and is the transport the SDK's own docs recommend for "remote
  servers, production deployments" — closer to how a real eviivo-hosted MCP server would run.
- **SSE** (legacy) isn't used by either half of the day — current SDK guidance is that
  Streamable HTTP is the modern recommended HTTP transport and SSE is legacy/being phased out.
  The course's own Day 3 material is the place for anything SSE-specific.

## Project structure, and why it's shaped this way

- **`Services/IRatesGateway.cs`, `IBookingGateway.cs`** — the interfaces a *real* eviivo
  integration would implement. `InMemoryRatesGateway` / `InMemoryBookingGateway` are
  fictional, deterministic stand-ins registered in `Program.cs`. Swapping in a real
  implementation means changing one line in `Program.cs` — nothing in `Tools/` changes. This
  mirrors the "no rewrite needed" framing from the morning slides.
- **`Models/`** — the structured result types, with `[Description]` on every field. This is
  "schema as agent documentation": the agent reads these descriptions the same way it reads the
  tool description. `FromGatewayResponse(...)` is the thin translation layer between the
  internal gateway shape and the agent-facing shape.
- **`Tools/`** — thin `[McpServerToolType]` classes. Each method is a few lines: call the
  gateway, map to the result type. All the actual logic lives in `Services/`, which is what you
  want if multiple tools (or a future REST endpoint) need to share the same business logic.

## Building on this scaffold (Day 2 afternoon)

This is meant to be a working example to extend, not just to watch. To add a new tool:

1. Define (or extend) a gateway interface + fictional in-memory implementation in `Services/`,
   following the `IRatesGateway` / `InMemoryRatesGateway` pattern.
2. Define a structured result record in `Models/`, with `[Description]` on every field.
3. Add a `[McpServerToolType]` class in `Tools/` with a constructor-injected gateway and one
   `[McpServerTool]` method, following `BookingLookupTool.cs` as the simplest template.
4. Register the gateway as a singleton and call `.WithTools<YourNewTool>()` in `Program.cs`.
5. Restart the server — `list_tools()` will show the new tool immediately.

If your squad is building against the `day2-afternoon/templates/*-starter` projects instead
(stdio transport, separate solution), the same four-step shape applies — only the transport
registration in `Program.cs` differs (`WithStdioServerTransport()` instead of
`WithHttpTransport(...)` + `app.MapMcp()`).

## Day 2 afternoon starter template review

While building this, I reviewed the three squad starters under `day2-afternoon/templates/`
(`connectivity-starter`, `core-platform-starter`, `web-ecommerce-starter`) as requested, since
attendees will build on top of them in the afternoon. One issue found and fixed, plus two stale
references cleaned up:

- **NuGet version pin bug (high priority — would have broken the lab for all 15 engineers).**
  All three starters' `.csproj` files pinned the base `ModelContextProtocol` package at
  `Version="1.4.0"`. Verified directly against the live NuGet feed: the highest version actually
  published for that package is **1.3.0** — 1.4.0 doesn't exist for the base package (only for
  `ModelContextProtocol.AspNetCore`, whose 1.4.0 release depends on `ModelContextProtocol >=
  1.4.0` — an unsatisfiable floor). Any squad starting from the original pin would have hit a
  `dotnet restore` failure on minute one. Fixed all three `.csproj` files, plus the matching
  claim in `templates/README.md`, to pin 1.3.0 — the highest version where both packages'
  dependency graphs actually agree. This same finding determined the version pin used in this
  demo (`ModelContextProtocol.AspNetCore` 1.3.0 for the Server project, `ModelContextProtocol`
  1.3.0 for DemoClient).
- **Stale `--prerelease` flag.** `ModelContextProtocol` is out of prerelease (1.3.0 is a full
  stable release), so the `dotnet add package ModelContextProtocol --prerelease` instruction in
  `day2-afternoon/README.md`, `slides.md`, and two spots in `facilitator-guide.md` (including a
  blocker-troubleshooting row) no longer applies — and would now needlessly pull in whatever the
  newest prerelease build is instead of the stable one. Dropped the flag in all four places.
- No functional/logic bugs found in the three starters' `Program.cs`, tool classes, or fictional
  API client stubs — DI registration order and `WithStdioServerTransport().WithToolsFromAssembly()`
  wiring all looked correct. I did deliberately reuse the same fictional ID formats those
  starters already use (`PROP-04821`/`PROP-09142` property IDs, `EVV-2026-NNNNN` booking
  references) in this demo's in-memory data, so the morning and afternoon feel like one
  continuous fictional world rather than two unrelated examples.

## A small inconsistency worth knowing about (not changed)

The Day 2 morning slides' JSON Schema slide for `get_rates_and_availability` uses snake_case
parameter names (`property_id`, `date_from`, `date_to`), while the adjacent C# code slide for
the same tool uses camelCase (`propertyId`, `dateFrom`, `dateTo`) — the names a C# method
signature would actually produce. This scaffold follows the C# slide (camelCase), since that's
the literal, runnable source and what a `[McpServerTool]` method actually generates. Flagging
this as something to reconcile in the slides themselves, not something I changed unilaterally.

## Security notes

- `AllowedHosts` in `appsettings.json` is restricted to loopback values
  (`localhost;127.0.0.1;[::1]`) rather than `*`, per the MCP C# SDK's own guidance — Kestrel
  doesn't validate the `Host` header by default, so a wildcard here is a DNS-rebinding risk even
  for a local demo.
- Stateless mode means there's no `Mcp-Session-Id` to manage and no in-memory session state to
  worry about across the description-toggle restarts.
- No auth is wired up — this is a local demo server, not something to expose past localhost as-is.

## Troubleshooting

| Symptom | Likely cause |
| --- | --- |
| `dotnet restore` fails on a version floor | Check both `ModelContextProtocol` and `ModelContextProtocol.AspNetCore` are pinned to versions whose dependency graphs agree — see the `.csproj` comments. Don't bump one without checking the other on nuget.org. |
| DemoClient can't connect | Is the server actually running (`dotnet run` in `EviivoRatesMcpServer`)? Does its console output show `http://localhost:5179`? |
| Toggling `UseVagueToolDescriptions` seems to do nothing | The server only reads config at startup — restart it after editing `appsettings.json`. |
| Unknown property/booking errors | Demo data only covers `PROP-04821`, `PROP-09142` and bookings `EVV-2026-00781`, `EVV-2026-00892` — see `Services/InMemoryRatesGateway.cs` / `InMemoryBookingGateway.cs` for the full fictional dataset. |
