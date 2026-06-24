# Day 2 Morning — MCP Concepts and Internal Design

**AI Engineering Intensive — eviivo, Islington, London**
**24–26 June 2026 | Day 2 Morning**

---

## Session overview

This morning builds the conceptual and architectural foundation for the afternoon lab. You will leave with a design sketch — tool names, input/output schemas, one Resource — that becomes the spec for the working MCP server you build after lunch.

---

## Exercise 1: Audit wall recap (individual, 5 minutes)

Before we start: look back at the audit board from Day 1.

Find one item your squad put in the "Dead Ends" or "Integrations" lane that an AI agent would need to use to do something useful.

Write down:
- What the agent would need to do
- What data it would need to do it
- Where that data currently lives in your architecture

Keep this in front of you for the morning.

---

## Exercise 2: Tool description quality (pairs, 10 minutes)

Here are two descriptions for the same MCP tool. Read both, then answer the questions below.

**Description A:**
```
Gets rate data.
```

**Description B:**
```
Returns current rates and room availability for a property across a date range.
Use this when you need to assess pricing, occupancy, or yield opportunities.
Returns rate tiers, channel restrictions, and available room count per night.
```

Questions:

1. An AI agent is deciding which tool to call to answer the question: "Are we overpriced for next weekend compared to last year?" Which description leads it to the right tool? Why?

2. What would an agent do if it received Description A and had three other tools available with similarly vague descriptions?

3. Write a one-sentence tool description for a tool from your squad's domain. Test it: if an agent read only that description, would it know when to use this tool — and when not to?

---

## Exercise 3: MCP server design sketch (pairs, 20 minutes)

Choose one eviivo API surface:

- **Connectivity internal API** — channel management for OTA integrations (Airbnb, Booking.com, Expedia, Vrbo)
- **Rates/Availability gateway** — room pricing, yield rules, stop-sell restrictions
- **eMail & Messaging API** — inbound/outbound guest messages, reply drafting, escalation
- **Booking gateway** — reservation creation, modification, cancellation, confirmation
- **eviivo external API** — what third-party systems call; the platform openness surface

Design your MCP server using this template. Work on paper or in a shared doc — you will use this in the afternoon lab.

---

### MCP Server design template

**Surface I'm wrapping:** ____________________

**The business question my server helps answer:**
```
(One sentence: "An agent should be able to answer: [X] using this server.")
```

---

**Tool 1:**

| Field | Your answer |
|---|---|
| Tool name | (verb_noun format, e.g. `get_channel_availability`) |
| One-line description | (written for an AI agent, not a developer) |
| When should the agent use this? | |
| When should the agent NOT use this? | |

Input schema — list the fields:

| Field name | Type | Required? | Description (for the agent) |
|---|---|---|---|
| | | | |
| | | | |
| | | | |

Output schema — what does the agent get back?

| Field name | Type | Description (what the agent should understand about this field) |
|---|---|---|
| | | |
| | | |
| | | |

---

**Tool 2 (if needed):**

| Field | Your answer |
|---|---|
| Tool name | |
| One-line description | |

Input fields (key ones only):

Output fields (key ones only):

---

**Tool 3 (if needed):**

| Field | Your answer |
|---|---|
| Tool name | |
| One-line description | |

---

**Resource:**

What context should be pre-loaded into the agent before it starts using this server? This is data that changes rarely and is expensive to re-fetch on every call.

```
Resource name: ____________________
Contents: (describe in plain English what the agent should have available)
Format: (structured JSON / plain text / key-value list)
```

---

**One constraint in a tool description:**

Identify one operational rule you want the agent to follow when using this server (e.g. call frequency, ordering, mandatory pre-conditions). Write it as a sentence inside a tool description.

```
"[Your constraint here — written in the tool description field, addressed to the agent]"
```

---

**The test:**

Could an agent with only this server answer the business question you wrote at the top? If no — what's the gap? Add a tool or Resource to close it.

---

## Exercise 4: Debrief contribution (individual, 2 minutes preparation)

Before the full-group debrief, decide:

1. **One tool name and description** from your sketch that you're confident about
2. **One design decision** you made (e.g. "we split this into two tools because...", "we put this in a Resource not a Tool because...")
3. **One thing you're uncertain about** — a decision you had to make without enough information

You will share these three things in the debrief. One minute per pair.

---

## Reference: MCP concepts at a glance

| Concept | What it is | eviivo example |
|---|---|---|
| MCP Server | A service that exposes Tools, Resources, and Prompts to any MCP client | A C# service wrapping your Rates/Availability gateway |
| Tool | A callable function with typed input/output schemas and an agent-facing description | `get_rates_and_availability(property_id, date_from, date_to)` |
| Resource | Data pushed into the agent's context — read, not called | Property config, tone profile, business rules |
| Prompt | A reusable prompt template offered by the server | Standard guest reply template with brand voice |
| Input schema | JSON Schema defining what the agent must send | `property_id: string`, `date_from: date`, `date_to: date` |
| Output schema | Typed return with field-level descriptions the agent reads | `sentiment_score: float`, `recommended_action: enum` |
| Tool description | The text field an agent reads to decide which tool to call | The most important field you will write |

---

## After the morning

Your design sketch from Exercise 3 is the input to the afternoon lab. Hold onto it.

In the afternoon you will:
1. Implement one tool from your sketch as a working C# MCP server
2. Connect it to a Claude agent
3. Have the agent answer a real business question using only your server's tools and resources

The quality of this afternoon depends on the quality of what you design this morning.
