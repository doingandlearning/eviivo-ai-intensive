---
title: "**Agentic Patterns and Skills Done Right**"
sub_title: AI Engineering Intensive — Day 1 Afternoon
author: Kevin Cunningham
---

## A guest complaint at 2am

The eviivo Concierge bot receives a message: *"There's mould in our bathroom and my kids are sleeping in it. We're leaving in the morning."*

<!-- pause -->

The bot logs the message. The inbox shows it as unread. No one sees it until 9am.

<!-- pause -->

**Was this a technology failure, a workflow failure, or a design failure?**


We'll return to this at the end of the session.

<!-- end_slide -->

<!-- jump_to_middle -->

Part 1 — Core Agentic Patterns
===

<!-- end_slide -->

## What makes a system "agentic"?

A chatbot responds. An agent **acts**.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Chatbot**

- Receives a message
- Generates a reply
- Stops

<!-- column: 1 -->

**Agent**

- Receives a trigger (message, event, schedule)
- Decides what to do
- Uses tools to do it
- Observes the result
- Decides what to do next

<!-- reset_layout -->

<!-- pause -->

The difference is not the model. It is the **loop**.

<!-- end_slide -->

## Pattern 1: Tool Use

The model decides which tool to call and what arguments to pass. You define the tools. The model does the routing.

<!-- pause -->

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

**How it works:**

1. System prompt describes available tools
2. Model generates a `tool_use` block (structured JSON)
3. Your code executes the tool
4. Result is fed back as `tool_result`
5. Model continues reasoning

<!-- column: 1 -->

```json
{
  "type": "tool_use",
  "name": "get_reservation",
  "input": {
    "guest_id": "G-4421",
    "include_notes": true
  }
}
```

<!-- reset_layout -->

<!-- pause -->

**At eviivo:** Concierge bot calling `get_booking_details`, `update_room_status`, `send_guest_message`. Each is a discrete, testable function.

<!-- end_slide -->

## Pattern 1: The decision the model makes

The model doesn't "know" your tools exist by magic. **Name and description quality determines call quality.**

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Weak tool definition**

```
name: "process"
description: "Does stuff"
```

Model can't use this reliably. It will either hallucinate arguments or skip the tool entirely.

<!-- column: 1 -->

**Strong tool definition**

```
name: "escalate_guest_complaint"
description: "Creates a priority 
incident ticket and notifies the 
duty manager via SMS. Use when 
a guest complaint cannot be 
resolved autonomously."
```

<!-- reset_layout -->

<!-- pause -->

**For the 2am mould complaint — what tools would you define?**


<!-- end_slide -->

## Pattern 2: Planning Loops (ReAct)

**Reason → Act → Observe** — the model "thinks", does something, sees what happened, then "thinks" again.

<!-- pause -->

```
THOUGHT: The guest reported mould. I need to know if this property has had prior maintenance flags.

ACTION:  get_maintenance_history(property_id="P-112", days=90)

OBSERVE: Two open tickets: bathroom extractor fan (overdue), damp report from previous guest (unresolved).

THOUGHT: This is a repeat issue. Escalation required, not just acknowledgement.

ACTION:  escalate_to_operations(ticket_ids=["T-44", "T-51"], priority="HIGH")
```

<!-- pause -->


<!-- end_slide -->

## Pattern 2: Where loops go wrong

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Runaway loops**
No stop condition. Model keeps calling tools. Side effects accumulate. Costs spike.

**Fixable with:** max turns, explicit task completion signal, human-in-the-loop checkpoint.

<!-- column: 1 -->

**Stuck loops**
Tool returns an error. Model retries indefinitely. No escalation path.

**Fixable with:** retry budgets per tool, fallback actions, error observation handling.

<!-- reset_layout -->

<!-- pause -->

For an automated check-in workflow — where would you put the stop condition, and what triggers escalation to a human?


Sometimes the right answer requires working through intermediate steps — not just retrieving a result.

<!-- pause -->

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

**Chain of thought** forces the model to externalise intermediate steps before giving a final answer.

Adding `"think step by step"` or using an extended thinking model (Opus 4.8) changes output quality on complex decisions.

**When it matters for eviivo:**
- AI Pricing: reasoning across occupancy, lead time, competitor rates, and seasonal demand simultaneously
- Sentiment analysis: distinguishing venting from a genuine service failure
- Auto-reply drafts: adjusting tone based on prior conversation context

<!-- column: 1 -->

```
Without CoT:
"Recommended rate: £189"

With CoT:
"Occupancy is 72% (above 
threshold). Lead time is 
11 days (short). Comp set 
is £195–220. Last year 
same week: £204. 
Recommended: £199."
```

<!-- reset_layout -->

<!-- pause -->

The difference isn't the conclusion — it's the **auditability**.

<!-- end_slide -->

## Pattern 3: Forcing reasoning in practice

You don't always need a thinking model. You can prompt for it.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Prompt patterns that help:**

- `"Before answering, list what you know and what's uncertain."`
- `"Think step by step. Show your working."`
- `"What assumptions are you making? State them first."`
- Extended thinking (Opus 4.8) — budget tokens explicitly

<!-- column: 1 -->

**When NOT to force reasoning:**

- Simple retrieval tasks (tool call with clear output)
- High-throughput, low-stakes classification
- Real-time latency constraints

Reasoning costs tokens. Use it where the quality delta justifies it.

<!-- reset_layout -->

<!-- pause -->

**Does the AI Pricing system currently show its reasoning, or just the rate?**


<!-- end_slide -->

## Pattern 4: Handoffs

A single agent trying to do everything becomes hard to test, hard to debug, and expensive. Handoffs let you decompose.

<!-- pause -->

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

**When to hand off:**

<!-- incremental_lists: true -->
- The task requires a different tool set
- A different specialist context (guest comms vs. pricing vs. channel sync)
- Human approval is required before continuing
- The next step has different latency or cost characteristics

**Routing approaches:**
- Model-driven: parent agent decides which sub-agent to call
- Rule-based: condition in your orchestration code routes the task
- Hybrid: model routes, code validates

<!-- column: 1 -->

**eviivo example:**

```
Guest complaint received
        │
        ▼
  [Triage Agent]
        │
   ┌────┴────┐
   ▼         ▼
[Comms    [Ops
 Agent]    Agent]
   │         │
Reply     Ticket +
drafted   escalation
```

<!-- reset_layout -->

<!-- end_slide -->

## Mapping patterns to eviivo scenarios

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

| Scenario | Primary Pattern |
|---|---|
| Concierge auto-reply | Tool use + handoff |
| Check-in workflow | Planning loop |
| AI Pricing recommendation | Multi-step reasoning |
| Channel event sync (Airbnb → DB) | Tool use (event-driven) |
| Complaint → ops escalation | Handoff + planning loop |
| Sentiment-triggered alert | Tool use + handoff |

<!-- column: 1 -->

**What the morning audit surfaced:**
<!-- incremental_lists: true -->
- Skills that use tool use but have no stop condition
- Planning loops defined in prompts, not in architecture
- Reasoning that happens but isn't visible or auditable
- Handoffs that exist as separate prompts with no shared state

<!-- reset_layout -->

<!-- pause -->

**Back to the 2am complaint:** which combination of patterns closes the gap?


<!-- end_slide -->

<!-- jump_to_middle -->

Part 2 — Skills Done Right
===

<!-- end_slide -->

## What a Skill actually is

A Skill is a reusable, named, versioned unit of agent capability — a combination of:

<!-- incremental_lists: true -->

- A **system prompt** (context, persona, constraints)
- **Tool definitions** (what the agent can do)
- **Input/output contracts** (what it expects, what it returns)
- **Error handling** (what happens when things go wrong)
- **Governance metadata** (who owns it, what version, where it lives)

<!-- incremental_lists: false -->

<!-- pause -->

A Skill that exists only in one developer's head — or only in one prompt file with no version history — is a liability, not an asset.

<!-- end_slide -->

## The Skills rubric

Every Skill the team writes should answer these questions:

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**1. Name clarity**
Does the name tell you what it does and when to use it — without reading the contents?

<!-- pause -->

**2. Input/output definition**
Are inputs typed and validated? Is the output shape documented?
<!-- pause -->
**3. Context sufficiency**
Does the system prompt give the model enough context to succeed without guessing?
<!-- pause -->
<!-- column: 1 -->

**4. Error handling**
What does the Skill do when a tool call fails, or an input is out of range?
<!-- pause -->
**5. Testability**
Can you run this Skill against a known input and assert the output? Is there a test case?
<!-- pause -->
**6. Governance**
Who owns it? What version is it? Where is it stored? Who can change it?

<!-- reset_layout -->

<!-- pause -->

This rubric is your exercise sheet for the next 30 minutes.

<!-- end_slide -->

## Skills critique: what good looks like

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Skill with problems**

```markdown
# Reply to guest

You are a helpful assistant.
Reply to guests professionally.
Use the tools available.
```

Issues: no context, no tool list, no input contract, no error handling, no owner.

<!-- column: 1 -->

**Skill done right**

```markdown
# guest-reply-drafter v1.2
Owner: Guest Experience Squad
Last reviewed: 2026-05-01

## Context
You are the eviivo Concierge assistant 
for [property_name]. Reply in [language].
Tone: warm, professional, solution-focused.

## Tools available
- get_booking_details(reservation_id)
- get_property_faq(property_id)

## Input
reservation_id: string (required)
guest_message: string (required)

## On tool failure
If get_booking_details fails, acknowledge 
the guest and escalate — do not guess 
booking details.

## Test case
Input: guest_message="Is breakfast 
included?" → faq has the answer
Expected: reply_text answers from 
the FAQ, escalate_flag: false
```

<!-- reset_layout -->

<!-- end_slide -->

## Skills critique exercise (30 minutes)

**You have the Skills surfaced in this morning's audit.**

<!-- pause -->

**Step 1 (10 min):** Swap Skills with your pair. Apply the rubric — one score per dimension (1 = absent, 2 = partial, 3 = clear).

<!-- pause -->

**Step 2 (10 min):** Identify the single highest-impact gap. Write one sentence explaining why it matters in production.

<!-- pause -->

**Step 3 (10 min):** Report back. One pair per Skill — name the gap, name the consequence.




<!-- end_slide -->

<!-- jump_to_middle -->

Part 3 — Skills Lab
===

<!-- end_slide -->

## Lab: pick your challenge

**You have 60 minutes. Pick one.**

<!-- pause -->

<!-- column_layout: [1, 1, 1] -->

<!-- column: 0 -->

**Option A**
Rewrite a Skill from the morning audit to the agreed standard. Use the rubric as your acceptance criteria.

*Good for:* engineers who found a clearly broken Skill and want to fix it completely.

<!-- column: 1 -->

**Option B**
Extend an existing Skill with error handling and context. The Skill works, but it fails silently.

*Good for:* engineers who have a Skill that runs in production but has no safety net.

<!-- column: 2 -->

**Option C**
Write a new Skill for a gap identified this morning. Suggestions: sentiment analysis trigger, check-in workflow step, channel sync event handler.

*Good for:* engineers who identified a missing capability and want to define the interface.

<!-- reset_layout -->

<!-- pause -->

**Deliverable:** a Skill file (markdown or JSON) + a one-paragraph explanation of the decisions you made. Not code — the Skill definition.

<!-- pause -->

<!-- incremental_lists: true -->

1. **Can someone use this Skill without asking you any questions?** If they'd need to ask you something, that information belongs in the Skill.
2. **What happens if the first tool call fails?** If the answer is "nothing", add error handling.
3. **Could you write a test for this?** State one input → expected output pair.
4. **What version is this?** Who owns it? Where does it live in the repo?

<!-- incremental_lists: false -->

<!-- pause -->

These are not bureaucratic gates. They are the difference between a Skill that works in a demo and one that works at 2am on Christmas Eve.


<!-- end_slide -->

## Lab: debrief (15 minutes)

**Each group presents their Skill in 3 minutes:**

<!-- incremental_lists: true -->

- What was the gap you identified?
- What decision did you make about the input/output contract?
- What does your error handling do?
- What would you need to version and govern this?

<!-- incremental_lists: false -->

<!-- pause -->

**After each presentation:** one observation from the group, one question.


<!-- end_slide -->

<!-- jump_to_middle -->

Part 4 — Orchestration Frameworks
===

<!-- end_slide -->

## Frameworks: the landscape

You don't need to commit to a framework to be effective. You need to understand what problems they solve.

<!-- pause -->

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

| Framework | Strength | Stack fit |
|---|---|---|
| **LangChain** | Broad ecosystem, many integrations | Any |
| **AutoGen** | Multi-agent conversations, role-based | .NET via SDK |
| **Semantic Kernel** | First-class C# support, MS ecosystem | **Your stack** |
| **Claude Agent SDK** | Anthropic-native, simple loop model | Any |

<!-- column: 1 -->

**What they all provide:**

<!-- incremental_lists: true -->
- Tool/function registration
- Memory management
- Loop orchestration
- Agent-to-agent handoffs
- Observability hooks

**What they all require:**

- You understanding the patterns first

<!-- reset_layout -->

<!-- pause -->

A framework without pattern fluency just hides your confusion at greater scale.

<!-- end_slide -->

## Semantic Kernel: why it matters for your stack

You are a .NET C# shop with Microsoft tooling throughout. Semantic Kernel is built for this.

<!-- pause -->

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

**What it gives you:**
<!-- incremental_lists: true -->
- Native C# SDK — Skills as C# functions with attributes
- Plug-in model maps directly to Claude/OpenAI tool use
- Integration with Azure AI, GitHub Models
- Memory connectors for SQL Server (which you already run)
- Kernel function chaining = planning loops in code

**Relevant for eviivo:**
- Windows services with scheduled logic → Kernel + Planner
- RabbitMQ events → Kernel plugin triggers
- MS SQL as agent memory store

<!-- column: 1 -->

```csharp
[KernelFunction]
[Description("Get reservation 
details for a guest")]
public async Task<Reservation> 
GetReservation(
    [Description("Reservation ID")]
    string reservationId)
{
    return await _db
        .Reservations
        .FindAsync(reservationId);
}
```

<!-- reset_layout -->


<!-- end_slide -->

## Framework choice: the actual decision

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->
<!-- incremental_lists: true -->
**Use a framework when:**
- You need agent-to-agent orchestration at scale
- Memory management is non-trivial
- You want observability out of the box
- The team needs a shared vocabulary for agent architecture

**Stay framework-light when:**
- You're still learning the patterns
- Your use case is one agent, one loop
- You need to ship something in two weeks

<!-- column: 1 -->

**The real risk:**

Picking a framework and learning the framework instead of learning the patterns.

A developer who understands tool use, planning loops, multi-step reasoning, and handoffs can work effectively with any framework — or none.

A developer who knows LangChain but not the patterns is stuck when the framework doesn't fit.

<!-- reset_layout -->

<!-- pause -->

**Does eviivo currently have any agent infrastructure that would benefit from a framework wrapper — or are you still at the patterns stage?**


<!-- end_slide -->

<!-- jump_to_middle -->

Summary and Bridge
===

<!-- end_slide -->

## Back to 2am

The mould complaint arrived. No one saw it until 9am.

<!-- pause -->

**Now you have the vocabulary:**

<!-- incremental_lists: true -->

- **Tool use failure:** `escalate_duty_manager` wasn't defined — the bot had no tool to act with
- **Planning loop failure:** there was no loop — a single retrieval, no observe-decide cycle
- **Multi-step reasoning failure:** the bot didn't connect "mould" + "prior tickets" + "children" to "HIGH priority"
- **Handoff failure:** Comms and Ops were separate systems with no agent bridge

<!-- incremental_lists: false -->

<!-- pause -->

None of these are model failures. They are **architecture and Skills** failures. And all of them are fixable with what you built today.

<!-- end_slide -->

## What we covered

<!-- incremental_lists: true -->

1. **Core agentic patterns** — tool use, planning loops, multi-step reasoning, handoffs — mapped to real eviivo scenarios
2. **Skills rubric** — six dimensions that separate production Skills from prototype prompts
3. **Skills critique** — structured analysis of real Skills against the rubric
4. **Skills lab** — rewrite, extend, or author a Skill to the agreed standard
5. **Orchestration frameworks** — the landscape, Semantic Kernel's fit for your stack, and when to use frameworks vs. patterns

<!-- incremental_lists: false -->

<!-- end_slide -->

## Bridge to Day 2

**We've established:**

<!-- incremental_lists: true -->

- The patterns that make systems agentic
- The standard for writing Skills that work in production
- Where your current Skills have gaps

<!-- incremental_lists: false -->

<!-- pause -->

**Day 2:** Observability, Evaluation, and Production Readiness

Every agentic system you build tomorrow will be one you can audit — because you'll be instrumenting reasoning, logging tool calls, and writing evals before you deploy.

The Skills you wrote today are the first things you'll test tomorrow.

<!-- end_slide -->

<!-- jump_to_middle -->

Questions?
===
