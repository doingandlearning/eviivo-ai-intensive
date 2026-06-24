---
title: "**Building Internal MCP**"
sub_title: Day 2 Afternoon
author: Kevin Cunningham
---

<!-- jump_to_middle -->

**A guest left a negative review on Booking.com.**

By the time your team sees it, it's 14 hours old.

**How many manual steps sit between "review posted" and "support ticket opened"?**

We'll build the answer to that question this afternoon.

<!-- speaker_note: Give them 20 seconds. Don't fill the silence. You want them counting steps in their head — that's the gap the session closes. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 1 — From Design to Scaffold
===

<!-- end_slide -->

## What you designed this morning

This morning you mapped your API surface and identified the tools your MCP server needs to expose.

This afternoon you build them.

<!-- pause -->

**Agenda:**

| Block | Time | What happens |
|---|---|---|
| Scaffold demo | 20 min | Watch a C# MCP server come to life |
| Governance | 20 min | Ownership, versioning, schema debt |
| Workflow patterns | 20 min | Windows services + RabbitMQ + MCP |
| Skills + MCP composition | 15 min | The full chain |
| Lab | 60 min | Build from your morning design |
| Debrief | 15 min | One decision, one regret, per squad |

<!-- speaker_note: Don't dwell on this slide. It's a roadmap, not a lesson. Move quickly. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 2 — Scaffold Demo
===

<!-- end_slide -->

## The ModelContextProtocol NuGet package

One package. No framework magic.

```
dotnet new console -n EviivoMcp
cd EviivoMcp
dotnet add package ModelContextProtocol
dotnet add package Microsoft.Extensions.Hosting
```

<!-- pause -->

That's your entire scaffold.

<!-- speaker_note: Run this live in a VS Code terminal on Windows. Don't pre-prepare output — the install noise normalises the real process. -->

<!-- end_slide -->

## Program.cs — the minimal host

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
```

<!-- pause -->

Three decisions embedded here:

<!-- incremental_lists: true -->

- **stdio transport** — Claude Desktop and VS Code connect over stdin/stdout
- **WithToolsFromAssembly** — any class marked `[McpServerToolType]` is auto-registered
- **No HTTP, no auth** — internal tool, internal network

<!-- incremental_lists: false -->

<!-- speaker_note: The no-auth point matters for the DevOps engineer — it runs as a Windows service under a service account. That IS your auth boundary. Address it if it comes up. -->

<!-- end_slide -->

## A real tool: GetBookingStatus

```csharp
using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerToolType]
public class BookingTools
{
    private readonly IEviivoApiClient _api;

    public BookingTools(IEviivoApiClient api) => _api = api;

    [McpServerTool, Description(
        "Returns the current status of a booking by reference number. " +
        "Use when a guest reports a problem and you need to verify their booking state.")]
    public async Task<BookingStatus> GetBookingStatus(
        [Description("The eviivo booking reference, e.g. EVV-2024-00123")]
        string bookingRef)
    {
        return await _api.GetBookingStatusAsync(bookingRef);
    }
}
```

<!-- speaker_note: Point at the [Description] attribute. This is the schema — it's what Claude reads. A vague description = a tool Claude won't use correctly. That connects directly to the schema debt conversation from Day 1. -->

<!-- end_slide -->

## What VS Code sees

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

When you run `dotnet run`, the MCP server starts listening on stdio.

Point your `claude_desktop_config.json` or VS Code MCP config at the process:

```json

```

<!-- column: 1 -->

Claude now sees:

- `GetBookingStatus`
- its description
- its parameter schema

...all derived from the C# attributes.

<!-- reset_layout -->

**Demo:** open the MCP inspector, call `GetBookingStatus` with a stub booking ref, show the JSON response.

<!-- speaker_note: If the demo fails to connect, don't paper over it — debug live. "This is exactly what you'll hit on day one" is worth more than a clean demo. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 3 — Governance and Team Ownership
===

<!-- end_slide -->

## Who owns this server?

<!-- column_layout: [2, 3] -->

<!-- column: 0 -->

**The question teams avoid:**

When a tool description is wrong and Claude hallucinates an action — whose bug is it?

<!-- pause -->

<!-- column: 1 -->

| Team size | Ownership model |
|---|---|
| 1–3 engineers | One owner, PR reviews |
| 4–10 (squad) | Squad owns their server; platform reviews schema changes |
| Platform team | Centralised registry; squads propose tools via PR |

<!-- reset_layout -->

<!-- pause -->

**For eviivo right now:** each squad owns their server.

<!-- speaker_note: This is a live decision for the room. The Connectivity lead owns the Connectivity MCP. The Core Platform lead and back-end architect own Rates/Availability MCP. The eCommerce squad owns the booking-engine MCP. If they push back on decentralisation, acknowledge it — a platform registry is a Day 3 conversation. -->

<!-- end_slide -->

## Versioning and schema debt

The `[Description]` attribute **is** your tool's contract.

Change it carelessly and every prompt that worked last week breaks silently.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Treat tool schemas like public APIs:**

- Semver: `1.0.0` → breaking tool signature change → `2.0.0`
- Non-breaking: add optional parameters, improve descriptions → `1.1.0`
- Keep a `CHANGELOG.md` per server

<!-- column: 1 -->

**The schema debt problem:**

A vague description written at 4pm on a Friday becomes a bug six months later when a new engineer asks Claude to "update the booking" and the wrong tool fires.

<!-- reset_layout -->

<!-- pause -->

**Rule:** if you can't write a one-sentence description that tells Claude *when* to call this tool and *when not to*, the tool isn't ready.

<!-- speaker_note: This connects to the Day 1 conversation about context debt. the AI Director may have opinions here — he's worth calling on. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 4 — Workflow Automation Patterns
===

<!-- end_slide -->

## Three patterns, one stack

<!-- incremental_lists: true -->

- **Scheduled task** — Windows Task Scheduler or a hosted .NET `BackgroundService` calls an MCP tool on a timer
- **Event trigger** — a RabbitMQ consumer wakes up on a message, calls the MCP server, acts on the result
- **Orchestration** — an agent loop calls multiple MCP tools in sequence, branching on results

<!-- incremental_lists: false -->

<!-- pause -->

Your stack already has all three of these.

**Windows services + RabbitMQ + an MCP server = a triggered agentic workflow.**

<!-- speaker_note: DevOps and back-end, this section is for you. Ask them "where in your current Windows service landscape would an MCP call slot in?" before moving to the next slide. -->

<!-- end_slide -->

## RabbitMQ-triggered workflow

```
Guest posts review on Booking.com
        │
        ▼
Connectivity Service → publishes ReviewReceived message to RabbitMQ
        │
        ▼
ReviewProcessor (Windows Service / BackgroundService)
  ├─ consumes ReviewReceived
  ├─ calls MCP tool: AnalyseSentiment(reviewText)
  ├─ if sentiment < threshold:
  │    calls MCP tool: CreateSupportTicket(bookingRef, summary)
  └─ publishes TicketCreated to RabbitMQ
```

<!-- pause -->

The MCP server doesn't know about RabbitMQ. The Windows service doesn't know about Claude.

**The boundary is clean by design.**

<!-- speaker_note: Draw attention to the decoupling. The MCP server is a capability layer. The orchestration logic lives in the Windows service. That's intentional — it makes the MCP server testable in isolation. -->

<!-- end_slide -->

## The BackgroundService pattern (.NET)

```csharp
public class ReviewProcessorService : BackgroundService
{
    private readonly IModel _rabbitChannel;
    private readonly IMcpClient _mcp;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var consumer = new AsyncEventingBasicConsumer(_rabbitChannel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var review = Deserialise<ReviewReceived>(ea.Body);

            var sentiment = await _mcp.CallToolAsync(
                "AnalyseSentiment",
                new { text = review.Content });

            if (sentiment.Score < 0.4)
            {
                await _mcp.CallToolAsync(
                    "CreateSupportTicket",
                    new { bookingRef = review.BookingRef,
                          summary = sentiment.Summary });
            }
        };
        _rabbitChannel.BasicConsume("review.received", false, consumer);
        await Task.Delay(Timeout.Infinite, ct);
    }
}
```

<!-- speaker_note: Don't read this out. Give them 30 seconds to read it themselves. Ask "what would you change for your squad's scenario?" -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 5 — Skills + MCP: The Full Chain
===

<!-- end_slide -->

## How they compose

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

A **Skill** is Claude's reusable workflow definition.

An **MCP server** is the capability layer — the tools Claude can call.

Together:

```
Skill: "Handle negative review"
  │
  ├─ Step 1: call AnalyseSentiment tool
  │          (MCP → internal NLP service)
  │
  ├─ Step 2: if negative, call CreateSupportTicket
  │          (MCP → eviivo ticketing API)
  │
  └─ Step 3: call NotifyTeam
             (MCP → Messaging gateway)
```

<!-- column: 1 -->

**The full chain:**

1. RabbitMQ message arrives
2. Windows service calls Claude with the Skill
3. Claude calls MCP tools in sequence
4. Results are written back to your system

**No new infrastructure.** Your existing Windows services become AI-capable.

<!-- reset_layout -->

<!-- speaker_note: This is the "aha" moment the session has been building toward. Pause after showing this. Ask "which of your current Windows services would you drop this into first?" -->

<!-- end_slide -->

## Returning to the opening question

**A guest left a negative review on Booking.com.**

You now have the pieces:

<!-- incremental_lists: true -->

- A RabbitMQ consumer that fires on `ReviewReceived`
- An MCP tool `AnalyseSentiment` backed by your NLP service
- An MCP tool `CreateSupportTicket` backed by your internal API
- A Skill that orchestrates the response
- A Windows service that wires them together

<!-- incremental_lists: false -->

<!-- pause -->

**How many manual steps sit between "review posted" and "ticket opened"?**

Zero.

<!-- speaker_note: Land this before moving to the lab. It's the payoff for the opening provocation. Don't rush past it. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 6 — Lab
===

<!-- end_slide -->

## Squad tracks — 60 minutes

Pick up your morning design. You know what to build.

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Connectivity Squad**
Connectivity Squad members

Scaffold an MCP wrapper for the Connectivity internal API. Start with one tool: `GetChannelSyncStatus(propertyId, channel)`.

Goal: working `dotnet run`, tool visible in MCP inspector.

<!-- column: 1 -->

**Core Platform Squad**
Core Platform Squad members

Scaffold a Rates/Availability MCP server. Add one tool: `GetRateAvailability(propertyId, dateRange)`. Stretch: wire a `BackgroundService` that polls on a schedule.

Goal: working server + one RabbitMQ-triggered pattern sketched in code.

<!-- reset_layout -->

<!-- pause -->

**Web/eCommerce Squad**
Web/eCommerce Squad members

Scaffold an MCP server for the booking engine. Start with `GetGuestConversation(bookingRef)`. Stretch: add `AnalyseSentiment` backed by a stub. Goal: full chain from guest message to support action in code.

<!-- speaker_note: Circulate. The Connectivity squad will hit auth questions first — remind them stdio transport + service account IS their auth. Core Platform may ask about hosting; the DevOps engineer knows the deployment story. Web/eCommerce squad may go too broad — keep them focused on one end-to-end chain. -->

<!-- end_slide -->

## Lab constraints

Three rules for the next 60 minutes:

<!-- incremental_lists: true -->

- **One working tool beats five stubs.** Ship something callable before adding more tools.
- **The `[Description]` attribute is not optional.** Write it before writing the implementation.
- **If you're blocked for more than 10 minutes, ask.** Don't debug in silence.

<!-- incremental_lists: false -->

<!-- speaker_note: Set a 30-minute checkpoint. Ask each squad "what's your working tool?" If a squad is stuck on setup, pair them with a senior engineer. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 7 — Debrief
===

<!-- end_slide -->

## Each squad: two things

**One decision you made** — what was it, and why?

**One thing you'd do differently** — not "we'd have more time," something architectural.

<!-- pause -->

**(Not a show-and-tell. We're looking for the hard calls.)**

<!-- speaker_note:
Go in order: Connectivity → Core Platform → Web/eCommerce.
Prompt if needed: "What did you argue about?" or "What did you cut to get something working?"
The AI Director — if they offer perspective, let them. Their experience is useful here.
Don't let debrief run over — Day 3 morning starts with External MCP and the room needs energy.
-->

<!-- end_slide -->

## Summary

<!-- incremental_lists: true -->

1. **Scaffold is fast** — one NuGet package, one host, one attribute per tool
2. **The description attribute is the contract** — write it like a public API, version it like one
3. **Your existing stack composes** — Windows services + RabbitMQ + MCP = triggered agentic workflows, no new infrastructure
4. **Skills orchestrate, MCP tools execute** — the boundary between them is where you put your logic

<!-- incremental_lists: false -->

<!-- end_slide -->

## Day 3 Morning: External MCP and Platform Openness

**We've established:**

<!-- incremental_lists: true -->

- How to build and govern MCP servers for internal use
- How to compose them with existing Windows service and messaging infrastructure
- How a Skill + MCP chain closes the gap between an event and an action

<!-- incremental_lists: false -->

**Day 3 Morning: External MCP and Platform Openness** — what happens when you expose MCP servers beyond your internal network? Partner integrations, the Connectivity layer as an MCP surface, and the governance model that changes when external clients can call your tools.

The ownership and versioning decisions you made today become significantly more consequential tomorrow.

<!-- end_slide -->

<!-- jump_to_middle -->

Questions?
===
