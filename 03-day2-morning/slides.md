---
title: "**MCP: From APIs to Architecture**"
sub_title: AI Engineering Intensive — Day 2 Morning
author: Kevin Cunningham
---

## You have 47 REST APIs. An AI agent calls none of them.

Your Rates/Availability gateway handles millions of price lookups a day. It is fast, documented, and battle-tested.

A Claude agent trying to answer "what's the best rate to offer this guest right now?" cannot call it.

<!-- pause -->

**Why not? [A] Security — agents shouldn't hit production APIs directly  [B] Protocol — agents don't speak REST the way a frontend does  [C] Context — the agent doesn't know what questions to ask**


We'll come back to this at the end of the morning.

<!-- end_slide -->

<!-- jump_to_middle -->

Part 1 — What Is MCP?
===

<!-- end_slide -->

## The fragmentation problem MCP was built to solve

Before MCP, every AI tool that wanted to call an external system had to invent its own integration:

<!-- incremental_lists: true -->

- OpenAI function calling — its own schema format
- LangChain tools — its own wrapper pattern
- Anthropic tool use — its own JSON structure
- Claude Skills — eviivo's own adapter layer

<!-- incremental_lists: false -->

<!-- pause -->

The result: every integration is bespoke. Nothing is reusable. Every new AI tool means rebuilding the same plumbing.

<!-- pause -->

**MCP — the Model Context Protocol — is Anthropic's answer:** a standardised open protocol that any AI client can use to discover and call any tool, regardless of which model or framework is driving the agent.


<!-- end_slide -->

## MCP from first principles: what the protocol actually does

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

MCP defines a **client-server model** for AI tool use:

<!-- incremental_lists: true -->

- **MCP Server** — exposes capabilities (tools, resources, prompts) to any MCP-compatible client
- **MCP Client** — an AI agent or host that discovers what a server offers and calls it
- **Tool discovery** — the client asks "what can you do?" and the server responds with a schema
- **Tool invocation** — the client sends a typed request; the server executes and returns structured output
- **Context injection** — servers can push context into the agent's window via Resources

<!-- incremental_lists: false -->

<!-- column: 1 -->

```
MCP Client (Claude agent)
        │
        │  1. list_tools()
        │  ──────────────▶
        │
        │  2. ← tool schema list
        │
        │  3. call_tool("get_rate", {...})
        │  ──────────────▶
        │
        │  4. ← structured result
        ▼
MCP Server (eviivo rates API)
```

<!-- reset_layout -->


<!-- end_slide -->

## The three things an MCP server exposes

<!-- column_layout: [1, 1, 1] -->

<!-- column: 0 -->
<!-- pause --> 
**Tools**

Functions the agent can invoke. Each tool has:
- A name
- A description (the agent reads this)
- An input schema (JSON Schema)
- An output schema

*Example: `get_availability`, `create_booking`, `send_guest_message`*
<!-- pause -->
<!-- column: 1 -->

**Resources**

Data the agent can read — pushed into context without a function call.

*Example: property config, guest preferences, current booking state*

Think of these as "things the agent should know before it starts."
<!-- pause -->
<!-- column: 2 -->

**Prompts**

Reusable prompt templates the server offers.

Less commonly used, but useful for standardising how agents interact with a domain.

*Example: a standard guest-reply template with your brand voice baked in*

<!-- reset_layout -->

<!-- pause -->

**Which of these three does eviivo need most urgently? [A] Tools — callable actions  [B] Resources — context injection  [C] Prompts — standardised interactions**


<!-- end_slide -->

<!-- jump_to_middle -->

Part 2 — MCP vs REST vs Skills
===

<!-- end_slide -->

## What MCP adds, what it doesn't replace




| | REST API | Skills | MCP Server |
|---|---|---|---|
| **Called by** | Frontend, services, integrations | Claude agent directly | Any MCP-compatible AI client |
| **Discovery** | Docs/OpenAPI spec | Registered in Claude | Runtime schema negotiation |
| **Auth** | API keys, OAuth | Platform-managed | Server-configured |
| **Output format** | Whatever you define | Structured for the model | JSON Schema — typed |
| **Context injection** | N/A | System prompt | Resources mechanism |
| **Who maintains** | Your backend team | Your AI team | Your backend team |

<!-- pause -->

**The key question isn't "which one" — it's "at which layer."**

<!-- pause -->

REST stays. Skills stay. MCP is the **adapter layer** that makes your existing REST surface callable by AI agents — with typed schemas, discoverability, and context injection baked in.

<!-- reset_layout -->


<!-- end_slide -->

## When to use each — the decision matrix

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Use REST when:**

<!-- incremental_lists: true -->

- A human-written service or frontend is the caller
- The caller knows exactly which endpoint to hit
- You need standard HTTP tooling (caching, CDN, load balancers)
- External partners need to integrate with you

<!-- incremental_lists: false -->

**Use Skills when:**

<!-- incremental_lists: true -->

- Claude is doing the reasoning and acting directly
- The task is self-contained within a single prompt/workflow
- You don't need runtime discovery — you know the shape upfront

<!-- incremental_lists: false -->

<!-- column: 1 -->

**Use MCP when:**

<!-- incremental_lists: true -->

- An AI agent needs to decide at runtime which tool to call
- Multiple AI clients need the same capability (not just Claude)
- You want to standardise how any AI consumes your platform
- The tool's availability or schema might change and the agent should adapt

<!-- incremental_lists: false -->

**The eviivo picture:**

Your REST APIs power the platform. Skills give Claude direct capabilities. MCP is the **public interface for AI** — the thing that makes your platform consumable by any AI client, now and in 18 months when the landscape has shifted again.

<!-- reset_layout -->


<!-- end_slide -->

<!-- jump_to_middle -->

Part 3 — From eviivo REST to MCP
===

<!-- end_slide -->

## Let's translate a real API: the Rates/Availability gateway

Your Rates/Availability gateway exposes (broadly):

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->
<!-- incremental_lists: true -->
**What the REST API does:**
- `GET /rates?property={id}&dates={range}` — fetch current rates
- `GET /availability?room={id}&dates={range}` — check room availability
- `PUT /rates` — update rate for a date range
- `GET /restrictions?property={id}` — fetch min-stay/stop-sell rules
- `POST /yield-rule` — apply a yield adjustment

<!-- column: 1 -->

**What an AI agent needs to be able to do:**
- "What's our occupancy looking like for this weekend?"
- "Are we competitively priced against the OTAs for next month?"
- "Apply the standard last-minute discount for rooms still available in 48 hours"
- "Flag if any property has stop-sell rules active that shouldn't be"

<!-- reset_layout -->

<!-- pause -->

These are different shapes of question. The agent needs to compose multiple REST calls, reason about the results, and decide what to do next.

**That composition layer is where MCP lives.**


<!-- end_slide -->

## A real MCP tool definition — in JSON Schema

What the agent sees when it discovers your Rates/Availability MCP server:

```json
{
  "name": "get_rates_and_availability",
  "description": "Returns current rates and room availability for a property across a date range. Use this when you need to assess pricing, occupancy, or yield opportunities. Returns rate tiers, channel restrictions, and available room count per night.",
  "inputSchema": {
    "type": "object",
    "properties": {
      "property_id": {
        "type": "string",
        "description": "The eviivo property identifier"
      },
      "date_from": {
        "type": "string",
        "format": "date",
        "description": "Start of the date range (inclusive), ISO 8601 format"
      },
      "date_to": {
        "type": "string",
        "format": "date",
        "description": "End of the date range (inclusive), ISO 8601 format"
      },
      "include_channel_rates": {
        "type": "boolean",
        "description": "If true, returns rates broken down by OTA channel. Default: false.",
        "default": false
      }
    },
    "required": ["property_id", "date_from", "date_to"]
  }
}
```


<!-- end_slide -->

## The same tool definition — in C#


```csharp
[McpServerTool]
[Description("Returns current rates and room availability for a property across a date range. " +
             "Use this when you need to assess pricing, occupancy, or yield opportunities. " +
             "Returns rate tiers, channel restrictions, and available room count per night.")]
public async Task<RatesAvailabilityResult> GetRatesAndAvailability(
    [Description("The eviivo property identifier")]
    string propertyId,

    [Description("Start of the date range (inclusive), ISO 8601 format")]
    DateOnly dateFrom,

    [Description("End of the date range (inclusive), ISO 8601 format")]
    DateOnly dateTo,

    [Description("If true, returns rates broken down by OTA channel. Default: false.")]
    bool includeChannelRates = false)
{
    // Calls your existing IRatesGateway service — no rewrite needed
    var result = await _ratesGateway.GetRatesAndAvailabilityAsync(
        propertyId, dateFrom, dateTo, includeChannelRates);

    return RatesAvailabilityResult.FromGatewayResponse(result);
}
```

<!-- pause -->

The C# SDK for MCP (`ModelContextProtocol.Server`) wraps your existing service layer. **You are not rewriting your API. You are annotating it.**


<!-- end_slide -->

## What the output schema looks like — and why it matters

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

```csharp
public record RatesAvailabilityResult
{
    [Description("Rates and availability by date. " +
                 "Each entry covers one night.")]
    public required IReadOnlyList<NightlyRate> Nights { get; init; }

    [Description("True if any night in the range has " +
                 "a stop-sell restriction active.")]
    public bool HasStopSellActive { get; init; }

    [Description("Count of nights where available " +
                 "rooms drops below 20% of capacity.")]
    public int LowAvailabilityNightCount { get; init; }
}
```

<!-- column: 1 -->

**Why the descriptions on return fields?**

The agent reads these too.

A response object with `HasStopSellActive: true` tells the agent more than a raw `bool` — combined with the field description, it knows what to do next.

<!-- pause -->

This is **schema as agent documentation** — a new design discipline your team will need.

<!-- reset_layout -->


<!-- end_slide -->

<!-- jump_to_middle -->

Part 4 — The Context Question
===

<!-- end_slide -->

## The hardest architecture question in AI systems

Where does context live?

<!-- pause -->

An AI agent answering "what's the best rate to offer this guest right now?" needs:

<!-- incremental_lists: true -->

- **Guest history** — how many times they've stayed, their preferences, any complaints
- **Current booking state** — what they've booked, how far out, which channel
- **Property occupancy** — how full are we this weekend vs next
- **Competitor rates** — what the OTAs are showing for comparable rooms
- **Business rules** — min-stay restrictions, yield targets, blackout dates

<!-- incremental_lists: false -->

<!-- pause -->

**Where should that context live? [A] In the MCP server — it fetches what it needs  [B] In the agent's system prompt — pre-loaded before the task  [C] In the tool's output — each tool call returns everything the agent might need  [D] Distributed — different context at different layers**


<!-- end_slide -->

## The context placement decision matrix


| Context type | Best location | Why |
|---|---|---|
| Static config (property details, business rules) | MCP Resource or system prompt | Changes rarely; expensive to re-fetch |
| Session-specific state (current guest, booking) | Agent system prompt at task start | Scoped to one conversation |
| Real-time data (live rates, availability) | MCP Tool call | Must be fresh; can't be cached |
| Cross-session history (guest preferences) | MCP Resource (pulled on demand) | Stable but personalised |
| Computed/aggregated insights | Tool output | Built at query time, not stored |

<!-- pause -->
**The schema debt problem revisited:**

From Day 1 — your abbreviated field names (`res_id`, `prop_cd`, `gst_fn`) are AI liabilities.

<!-- pause -->

The MCP layer is where you fix this **without a schema migration.**
<!-- incremental_lists: true -->
Your MCP server translates:
- `res_id` → `reservation_id` in the tool output schema
- `prop_cd` → `property_code`
- `gst_fn` + `gst_sn` → `guest_full_name`

The LLM gets clean field names. Your database stays unchanged.

<!-- reset_layout -->


<!-- end_slide -->

## Context placement in practice: the Guest Communications case

A concrete design walk-through for guest communications work:

<!-- column_layout: [3, 2] -->
<!-- pause -->
<!-- column: 0 -->

**At agent startup (system prompt):**
```
Property: The Marlowe Hotel, Canterbury
Current date: 2026-06-26
Tone profile: warm-professional
Response SLA: 2 hours
Escalation threshold: sentiment_score < -0.4
```

<!-- pause -->
**At task time (MCP Tool call):**
```
get_guest_context(guest_id, booking_id)
→ guest_name, stay_dates, channel,
  prior_complaint_count, loyalty_tier,
  last_3_message_summaries
```

<!-- column: 1 -->

<!-- pause -->
**In the tool description:**
```
"Use get_guest_context before drafting 
any guest reply. Returns the minimum 
context needed to personalise the 
response. Do not call this more than 
once per conversation turn."
```

<!-- pause -->

**What this prevents:**

The agent re-fetching the full guest record on every message. The tool description is the constraint — the agent reads it and follows it.

<!-- reset_layout -->


<!-- end_slide -->

<!-- jump_to_middle -->

Part 5 — MCP and the Four Strategic Priorities
===

<!-- end_slide -->

## MCP maps to every priority on your roadmap

This is not a future capability. It's infrastructure for what you're already building.

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**1. Testing and Deployment**

MCP servers have typed schemas. Typed schemas are testable. Unit-test your tool definitions the same way you test your service layer.

**Design question:** What's the minimum tool schema for CI to catch a breaking change before it reaches the agent?

<!-- pause -->

**2. Guest Communications**

The sentiment analysis gap (no tone-of-voice configs, no agentic trigger on negative sentiment) is a missing MCP tool and a missing MCP resource — not a missing AI feature.

**Design question:** What does `analyse_guest_sentiment` return that triggers a workflow?

<!-- column: 1 -->

<!-- pause -->
**3. Pricing Model**

You're already running Claude Opus 4 for AI Pricing. The rates gateway MCP server is the bridge between Opus reasoning and your live data.

**Design question:** What does the pricing agent need that it can't safely get via a direct DB query?

<!-- pause -->

**4. Platform Openness**

Connectivity squad: an MCP server is a machine-readable API surface any AI client can consume. Your OTA integrations become AI-agent integrations with the same server.

**Design question:** What would it mean for Booking.com or Airbnb to call an eviivo MCP server rather than a REST endpoint?

<!-- reset_layout -->


<!-- end_slide -->

## The Guest Manager gap — a concrete MCP design

What's missing from the current eviivo guest communications architecture:

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Gap:** No structured sentiment analysis output that can trigger an agentic workflow.

**MCP design that closes it:**

```json
{
  "name": "analyse_message_sentiment",
  "description": "Analyses a guest message for sentiment, urgency, and complaint category. Returns a structured result that can trigger escalation workflows. Always call this before drafting a reply to an incoming guest message.",
  "inputSchema": {
    "properties": {
      "message_text": { "type": "string" },
      "booking_id": { "type": "string" },
      "property_id": { "type": "string" }
    }
  }
}
```

<!-- column: 1 -->
<!-- pause -->
**Output schema:**

```csharp
public record SentimentAnalysisResult
{
    [Description("Score from -1.0 (very negative) " +
                 "to 1.0 (very positive)")]
    public required float SentimentScore { get; init; }

    [Description("True if the message contains " +
                 "an explicit review threat")]
    public bool ReviewThreatDetected { get; init; }

    [Description("Category: maintenance | billing | " +
                 "noise | cleanliness | staff | other")]
    public required string ComplaintCategory { get; init; }

    [Description("Recommended action: reply | " +
                 "escalate | urgent-call")]
    public required string RecommendedAction { get; init; }
}
```

<!-- reset_layout -->


<!-- end_slide -->

## Demo

**Demo:** A working MCP server — tool discovery, tool invocation, structured response.

Show:
1. `list_tools()` — the agent discovers what's available
2. `call_tool("get_rates_and_availability", {...})` — a typed call with a property ID and date range
3. The structured response the agent receives
4. The agent composing a recommendation based on that response

<!-- pause -->

Then show what breaks when the tool description is poor — a vague description, the agent picks the wrong tool.


<!-- end_slide -->

<!-- jump_to_middle -->

Part 6 — Design Exercise
===

<!-- end_slide -->

## Design exercise: sketch your MCP server

**In pairs (20 minutes).** Each pair takes one eviivo API surface and sketches an MCP wrapper.

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

**Pick one surface:**

<!-- incremental_lists: true -->

- Connectivity internal API (channel management)
- Rates/Availability gateway (pricing)
- eMail & Messaging API (guest comms)
- Booking gateway (reservation management)
- eviivo external API (platform openness)

<!-- incremental_lists: false -->

**What to produce (on paper or shared doc):**

<!-- incremental_lists: true -->

- 3–5 tool names and one-line descriptions
- Input schema for your most important tool (field names, types, descriptions)
- Output schema for the same tool (what the agent actually needs)
- One Resource: what context should be pre-loaded?
- One constraint you'd encode in a tool description

<!-- incremental_lists: false -->

<!-- column: 1 -->

**The test:**

Ask yourself: if an agent only had access to this server, could it answer the most important business question in this domain?

<!-- pause -->

If no — what's missing from your tool list?

If yes — what could go wrong if the output schema is wrong?

<!-- reset_layout -->


Each pair: **one tool name, one design decision you made, one thing you're uncertain about.**

<!-- pause -->

Common patterns to surface:

<!-- incremental_lists: true -->

- Tools that are too broad (one tool that does five things — needs splitting)
- Tools that are too narrow (five tools that should be one)
- Missing context: the tool can't work without data that isn't in the input schema
- The description problem: the tool name makes sense to a developer but not to an agent
- The output problem: returning raw data when the agent needs a recommendation

<!-- incremental_lists: false -->


<!-- end_slide -->

## Authentication and security boundaries — the questions you must answer before building

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Authentication:**

<!-- incremental_lists: true -->

- Which MCP servers are internal-only (called by agents running inside your infrastructure)?
- Which are external (called by third-party AI clients)?
- Internal: service account tokens, scoped IAM roles
- External: OAuth 2.0, API keys, rate limiting, same as your existing external API

<!-- incremental_lists: false -->

<!-- pause -->

**The principle:**

An MCP server is a new API surface. Apply the same security posture you'd apply to any new REST API because it is one, with an AI client instead of a human one.

<!-- column: 1 -->

**Scope boundaries:**

<!-- incremental_lists: true -->

- A rates MCP server should not be able to create bookings
- A guest comms MCP server should not be able to modify rates
- Tool schemas enforce scope — if a tool doesn't exist, the agent can't call it
- Log every tool invocation — you need an audit trail for agent actions

<!-- incremental_lists: false -->

<!-- pause -->

**The question for the DevOps engineer:**

Where do MCP servers live in the eviivo deployment topology? Alongside your existing APIs? As a separate service tier? What does the CloudFront/private cloud boundary look like?

<!-- reset_layout -->


<!-- end_slide -->

<!-- jump_to_middle -->

Part 7 — Closing the Loop
===

<!-- end_slide -->

## Back to the opening question

**You have 47 REST APIs. An AI agent calls none of them. Why not?**

<!-- column_layout: [1, 1, 1] -->

<!-- column: 0 -->

**[A] Security**

Partly right. Direct REST hits from an agent bypass your normal auth patterns. MCP servers have explicit auth, explicit scope, and an audit trail. The agent calls the MCP server — not your raw API.

<!-- column: 1 -->

**[B] Protocol**

Right. Agents need runtime discovery, typed schemas, and structured outputs. REST endpoints designed for frontends don't have those. MCP adds the layer that makes your REST surface agent-callable.

<!-- column: 2 -->

**[C] Context**

Right. An agent calling your rates API needs to know what question to ask, in what format, and what the result means. That context lives in the tool description, the input schema, the output schema, and the Resources the server provides.

<!-- reset_layout -->

<!-- pause -->

**The answer is all three.** MCP addresses all of them — security boundaries by design, protocol by definition, and context by the schema discipline you build into every tool definition.


<!-- end_slide -->

## Summary

<!-- incremental_lists: true -->

1. **MCP is a protocol, not a product** — client-server, tool discovery, typed invocation, context injection; runs over any transport
2. **MCP doesn't replace REST or Skills** — it's the adapter layer that makes your REST surface agent-callable and your Skills composable
3. **Tool descriptions are agent documentation** — the quality of the description field determines which tool an agent picks; this is a new design discipline
4. **The MCP output schema is your translation layer** — clean field names, typed returns, and embedded recommendations without touching your database schema
5. **Context lives at multiple layers** — system prompt for static config, Resources for session context, Tool calls for real-time data
6. **Security is the same question as for any API** — scope, auth, audit trail; the client is an AI agent, not a browser

<!-- incremental_lists: false -->

<!-- end_slide -->

## Bridge to the afternoon

**We've established:**

<!-- incremental_lists: true -->

- What MCP is and why it exists — the protocol, the mental model, the client-server architecture
- How your REST API surface translates into MCP tool definitions, input schemas, and output schemas
- Where context lives and how the MCP layer resolves your schema debt
- The design sketches from the exercise — 3–5 tool definitions per domain area

<!-- incremental_lists: false -->

**This afternoon:** Building Internal MCP — you will implement one of those design sketches as a working MCP server in C#, connect it to a Claude agent, and have it answer a real business question against your domain.

The design decisions you made this morning are the architecture you will build this afternoon.


<!-- end_slide -->

<!-- jump_to_middle -->

Questions?
===
