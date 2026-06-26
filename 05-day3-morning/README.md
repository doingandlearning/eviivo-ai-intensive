# Day 3 Morning — External MCP and Platform Openness

**AI Engineering Intensive — eviivo, Islington, London**
**26 June 2026, morning session**

---

## Session overview

This session shifts from building MCP for internal use (Day 2) to the platform boundary: how eviivo exposes its capabilities to external AI clients, and how it consumes external AI services responsibly.

The session closes with a design exercise that should produce concrete backlog candidates.

**Duration:** approximately 3 hours including the design exercise and debrief

---

## Pre-reading (optional, 15 minutes)

If you want to arrive with context on the threat models discussed in Part 5:

- [Model Context Protocol specification — authentication section](https://modelcontextprotocol.io/specification/2025-03-26/basic/authorization)
- Your squad's current external API documentation (especially if you work on the Connectivity, Rates/Availability, or Messaging gateways)

---

## Design exercise

You will work in pairs or small groups on one of two tracks. Choose the track most relevant to your current squad or role.

---

### Track A: Guest Communications Workflow

**Scenario:** eviivo Concierge currently handles automated responses but has no sentiment signal, no tone configuration, and no workflow trigger. You are designing the MCP composition that fills this gap.

**Deliverable:** a design artefact (diagram, tool list, or structured notes) covering the following:

---

**Step 1: Define your tools**

List each MCP tool your composition needs. For each tool, specify:

| Field | What to include |
|---|---|
| Tool name | Use a verb + noun pattern (`AnalyseGuestSentiment`, not `Sentiment`) |
| Description | Write it as an external contract — include scope, rate limit hint, fallback behaviour |
| Inputs | Name, type, required/optional |
| Outputs | Name, type, what it means |
| External dependency? | If yes: what provider, what happens if it's unavailable? |

---

**Step 2: Draw the agent decision flow**

Sketch the flow from "guest message arrives" to "response sent or queued." Include:

- The tools called at each step (in order)
- The decision points (what conditions branch the flow?)
- What happens when a step fails
- Where a human can intervene

---

**Step 3: Identify risks**

List your two highest-risk points in the composition. For each:

- What is the risk? (technical failure, security, data privacy, or user experience)
- What is your mitigation?
- What monitoring would tell you the risk has materialised?

---

**Step 4: Tone configuration**

Define what `GetPropertyToneConfig` returns. Think about:

- What parameters does a property manager actually control?
- What is the data type and valid range for each?
- Who sets this? Through what UI?
- What is the sensible default for a property that has not configured it?

Write out a sample JSON response from this tool for a mid-range London hotel that wants warm, professional responses with escalation to a duty manager for high-urgency negative sentiment.

---

**Step 5: MVP scope**

You have one quarter. What is the minimum version of this workflow you would ship?

- Which tools are in scope?
- Which decisions are deferred to a future quarter?
- What does "done" look like — how would you know it's working?
- What dependencies do you need from other squads?

---

### Track B: Platform Openness — ISV Partner MCP Server

**Scenario:** eviivo wants to be more consumable by ISV partners and third-party developers via MCP. You are designing the external-facing MCP server that a partner would use to build a pricing or analytics tool.

**Deliverable:** a design artefact covering the following:

---

**Step 1: Define your tool surface**

List the MCP tools you would expose to ISV partners. For each tool, specify:

| Field | What to include |
|---|---|
| Tool name | Stable, versioned (`GetAvailabilityV1`) |
| Description | Contract statement: scope required, rate limit, version, deprecation date if applicable |
| Required scope | What OAuth scope does the caller need? |
| Inputs | Name, type, required/optional |
| Outputs | Name, type |
| Data classification | What data does this return? PII? Financial? Availability only? |

---

**Step 2: Authentication and onboarding**

Answer these questions:

- How does a partner get an API key? What is the onboarding journey?
- What scopes can a partner request, and who approves them?
- Is it OAuth 2.0 client credentials, API key + HMAC, or something else? Why?
- How do you revoke access if a partner's key is compromised?

---

**Step 3: Data residency**

For a UK-based ISV partner and a US-based ISV partner:

- What data can you return to each?
- Where are the constraints (GDPR, contractual, eviivo policy)?
- What do you strip or mask before the response leaves the Calligo boundary?

---

**Step 4: Versioning and deprecation**

- How do you version tools? (Method name suffix, URL path, header, or combination?)
- What is your deprecation policy? How much notice do you give a partner?
- What happens when a partner calls a deprecated tool after the sunset date?
- How do you communicate schema changes to partners proactively?

---

**Step 5: The partner experience**

- What would a partner build first with your tool surface?
- What documentation would they need?
- What sandbox/testing environment would they need?
- What is the difference between your MCP developer experience and your current REST API developer experience?

---

## Debrief preparation

After the exercise, each group will have 5 minutes to present:

1. Your tool list (names and scopes only — no need to read descriptions)
2. Your agent decision flow or partner onboarding flow (brief walkthrough)
3. Your single highest-risk point and your mitigation

Be ready to answer: **"What is the first thing you would actually build, and what does the ticket look like?"**

---

## Key terms

**MCP server** — a service that exposes tools to MCP-compatible AI agents via a structured protocol

**Tool description** — the text an LLM reads to understand what a tool does and how to call it; in external contexts, this is also a security surface

**Schema pinning** — storing a snapshot of an external tool's input/output contract and alerting when it drifts, rather than accepting whatever the external server sends

**Circuit breaker** — a pattern that stops calling a failing dependency after a threshold of failures, preventing cascade failures into your own system

**Scope** — an OAuth 2.0 concept defining what a token authorises; in external MCP, scopes limit what an ISV's agent can call on your behalf

**Tone configuration** — property-level settings that control the voice, formality, escalation thresholds, and auto-reply behaviour of the guest communications workflow

**Workflow trigger** — the connection between a guest message event (or sentiment signal) and an automated agent action; the link currently missing from eviivo Concierge
