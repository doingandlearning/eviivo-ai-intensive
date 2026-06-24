---
title: "**Where Are We Now?**"
sub_title: AI Engineering Intensive — Day 1 Morning
author: Kevin Cunningham
---

## You get a support ticket at 11pm

A guest messages the eviivo Concierge bot: *"The room smells like cigarettes, the shower is broken, and nobody has responded to my last three messages. I'm leaving a review."*

The bot sends a cheerful automated reply: *"Thanks for reaching out! We'll be in touch soon."*

<!-- pause -->

**Who owns this failure? [A] The prompt  [B] The workflow  [C] The missing context**

<!-- speaker_note: Hold for 30 seconds. Don't explain the options yet. You want split answers — all three are defensible and that's the point. Come back to this on the penultimate content slide. -->

We'll come back to this at the end of the morning.

<!-- end_slide -->

<!-- jump_to_middle -->

Part 1 — What Have You Built?
===

<!-- end_slide -->

## The audit starts now

Before any theory: let's get what exists out of people's heads and onto the wall.

<!-- pause -->

Three squads, three simultaneous contributions. You have **15 minutes**.

<!-- pause -->

**What to capture — one item per sticky:**

<!-- incremental_lists: true -->

- A Skill or prompt you use regularly (name it, describe what it does)
- A workflow that touches AI (even partially, even informally)
- An integration that calls an LLM or gets called by one
- Something you tried that didn't work — and your best guess why

<!-- incremental_lists: false -->


<!-- speaker_note: Pre-load the board with four swim lanes Skills/Prompts | Workflows | Integrations | Dead Ends. Have sticky templates ready. Give squads 15 minutes then call time — be firm. The messiness of this board is the raw material for the next two sections. -->

<!-- end_slide -->

## What just landed on the wall


<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Categories we expect to find:**

<!-- incremental_lists: true -->

- One-off prompts living in someone's notes app
- Skills written by one person, unknown to others
- Integrations with undocumented assumptions baked in
- Workflows that depend on a specific person's context
- Things that worked once but nobody knows why

<!-- incremental_lists: false -->

<!-- column: 1 -->

**The questions to ask of each:**

<!-- incremental_lists: true -->

- Is it documented anywhere?
- Does anyone else know it exists?
- Would it work if the person who built it left tomorrow?
- What context does it silently assume?

<!-- incremental_lists: false -->

<!-- reset_layout -->

<!-- speaker_note: Walk the board squad by squad. Don't evaluate yet — just classify. The goal is to surface the pattern things exist in isolation, assumptions are invisible, duplication is rife. Name what you see rather than asking for it. -->

<!-- end_slide -->

## What we're seeing — name the pattern

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

Every team that uses AI tools builds up the same kind of debt:

<!-- incremental_lists: true -->

- **Prompt debt** — one-off prompts tuned for one person, one context, one day
- **Context debt** — assumptions about your data model that are never written down
- **Documentation debt** — things that work but can't be shared, extended, or debugged
- **Duplication debt** — three people solved the same problem three different ways last month

<!-- incremental_lists: false -->

<!-- column: 1 -->

This is not a discipline problem.

<!-- pause -->

It's what happens when capable engineers move fast without shared standards.

<!-- pause -->

**The question is: what does "shared standards" actually mean for AI work?**

<!-- reset_layout -->

<!-- speaker_note: This should land as recognition, not criticism. You want nods. If someone pushes back ("we do document things"), ask "where does someone new go to find out what Skills exist?" That usually ends the debate gently. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 2 — Where Practice Varies
===

<!-- end_slide -->

## The consistency problem


<!-- pause -->

Compare your Squad's approach to one common task — writing a prompt for a guest-facing reply. What choices did you make that the other squad didn't? What would happen if you swapped prompts?

<!-- speaker_note: Pre-assign rooms Connectivity + Core Platform in one, Web/eCommerce alone or with facilitator check-in. The goal is not to find the "right" answer but to surface that different squads have different implicit standards. Listen for different context injection strategies, different persona assumptions, different tone decisions, different failure handling. Bring back one specific difference per room. -->

<!-- end_slide -->

## What variation looks like in practice

Three real patterns from teams like yours:

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Prompt A** (Connectivity squad pattern)
```
You are a helpful assistant. Reply to this 
guest message professionally.

Message: {{message}}
```

<!-- column: 1 -->

**Prompt B** (Core Platform squad pattern)
```
You are the eviivo Concierge for {{property_name}}.
Tone: {{tone_profile}}. 
Guest history: {{last_3_interactions}}.
Current booking status: {{booking_state}}.

Guest message: {{message}}

Reply in under 80 words. If sentiment is 
negative, flag for human review.
```

<!-- reset_layout -->

<!-- pause -->

**Which one is better? [A] A — simpler, more flexible  [B] B — more context, better output  [C] depends on what you're optimising for**

<!-- speaker_note: The right answer is C — but A and B are both defensible. The point is that Prompt B has explicit context injection, a tone constraint, a word limit, and a handoff condition. None of those choices are obvious. They represent decisions someone made. Without documentation, those decisions are invisible to everyone else. -->

<!-- end_slide -->

## The undocumented assumption problem

What Prompt B assumes that Prompt A doesn't:

<!-- incremental_lists: true -->

- `{{property_name}}` — is this field always populated? What's the fallback?
- `{{tone_profile}}` — who defines "tone"? Is it consistent across properties?
- `{{last_3_interactions}}` — what if there are none? What counts as an "interaction"?
- `{{booking_state}}` — where does this come from? What are the valid values?
- "negative sentiment" — who decided what negative means here? What model? What threshold?

<!-- incremental_lists: false -->

<!-- pause -->

These are not edge cases. They are the **shape of your data model** made visible.

When an LLM receives bad, missing, or misnamed context — it doesn't crash. It **hallucinates confidently**.

<!-- speaker_note: This is the hinge point. The goal is for the team to name this as a real problem they've seen, not an abstract one. Ask "has anyone seen the bot reply confidently with wrong information?" — you'll get stories. Those stories are this problem. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 3 — What Good Looks Like
===

<!-- end_slide -->

## Shared standards for AI work

Not a style guide. A set of decisions your team makes once, documents, and enforces.

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

**For Skills:**

<!-- incremental_lists: true -->

- Named consistently (verb-noun: `reply-guest-message`, `summarise-booking-notes`)
- Version-controlled alongside the code that calls them
- Include: purpose, inputs, outputs, failure modes, owner
- Tested — not just "it worked once"

<!-- incremental_lists: false -->

<!-- column: 1 -->

**For context injection:**

<!-- incremental_lists: true -->

- Document every field an LLM receives
- Name fields for the LLM, not just for your ORM
- Define fallbacks explicitly — no silent empties
- Log what was sent when something goes wrong

<!-- incremental_lists: false -->

<!-- reset_layout -->

<!-- speaker_note: Don't dwell here — this is a framing slide. The team will build their own version of this in the afternoon session. Plant the seed "we're going to build your team's version of this today." -->

<!-- end_slide -->

## A Skills registry — what it looks like in practice


<!-- pause -->

Minimum viable entry for a Skill:

```yaml
name: reply-guest-message
version: 1.2
owner: connectivity-squad
purpose: Generate a first-response to a guest message in the eviivo Concierge inbox
inputs:
  - message: string (the guest's raw message text)
  - property_name: string
  - tone_profile: enum [professional, friendly, formal] — default: friendly
  - booking_state: enum [pre-arrival, in-stay, post-stay, no-booking] — required
outputs:
  - reply_text: string (max 120 words)
  - escalate_flag: boolean
failure_modes:
  - empty message → return null, do not reply
  - unknown booking_state → escalate_flag: true, log warning
tested: true
last_tested: 2026-05-12
```

<!-- speaker_note: This is not prescriptive — the team will adapt this. The point is that a Skills registry doesn't need to be a sophisticated system. A YAML file in a shared repo is enough to start. The value is the discipline of writing it down, not the format. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 4 — The Context Problem
===

<!-- end_slide -->

## Your schema is an AI liability

An LLM is only as useful as the context it receives.

<!-- pause -->

eviivo's data model has evolved over many years. Consider what an LLM typically sees:

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**What your ORM calls it:**
```
res_id
prop_cd
gst_fn
gst_sn
arr_dt
dep_dt
rm_type_cd
src_cd
```

<!-- column: 1 -->

**What an LLM needs to reason about it:**
```
reservation_id
property_code
guest_first_name
guest_surname
arrival_date
departure_date
room_type_code
booking_source_code
```

<!-- reset_layout -->

<!-- pause -->

Abbreviated field names made sense for database performance in 2010. They are **active liabilities** in a context window in 2026.

<!-- speaker_note: Ask the room "what does your actual schema look like?" — you'll get laughs and recognition. Don't pile on. The point is not that the schema is bad engineering, it's that it was built before LLMs were part of the stack. The fix is a translation/enrichment layer, not a schema rewrite. -->

<!-- end_slide -->

## Context windows — what they are and why they matter

Not magic memory. A finite input buffer.

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

Every model call receives a context window — the complete text (tokens) the model can "see" at once.

<!-- incremental_lists: true -->

- **Claude Sonnet 4.6:** 200,000 tokens standard (up to 1M on extended context)
- **Claude Opus 4.8:** 200,000 tokens standard (up to 1M on extended context)
- **OpenAI's GPT-5 family:** large context windows too, varies by tier — check the current figure close to delivery, this moves fast
- **Rough rule:** 1 token ≈ 0.75 words in English

<!-- incremental_lists: false -->

<!-- pause -->

200,000 tokens sounds enormous. But consider what you're putting in that window:

<!-- column: 1 -->

**What eats your token budget:**

<!-- incremental_lists: true -->

- System prompt with all your property context
- Conversation history (grows with every turn)
- Retrieved booking/guest records
- Tool call results
- The actual question
- The response you're generating

<!-- incremental_lists: false -->

<!-- reset_layout -->

<!-- speaker_note: The key insight here token budget management is not an optimisation problem for later. It shapes architecture decisions right now — what to retrieve, what to summarise, what to cache. This becomes critical for scheduling/analytics work and data aggregation patterns. -->

<!-- end_slide -->

## Token budget decisions — a practical frame

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**What goes in the context window?**

| Content type | Token cost | Value |
|---|---|---|
| Full conversation history | High | Decreasing over time |
| Full guest record | Medium | Usually high |
| All property settings | Medium | Often low per call |
| Retrieved docs/policies | Variable | High when relevant |
| Summarised history | Low | Often sufficient |

<!-- column: 1 -->

**The design question is always:**

*What does the model need to know to do this specific task well — and what can be left out?*

<!-- pause -->

Not "what's available?" — that way lies expensive, slow, and unreliable pipelines.

<!-- pause -->

**Does your team currently think about token budget when building AI features? [A] Yes, explicitly  [B] Sometimes  [C] Not yet**

<!-- reset_layout -->

<!-- speaker_note: This is a temperature check. C is the most common answer — use it to motivate the afternoon session on architectural patterns. A is the answer you want more people giving by Day 3. -->

<!-- end_slide -->

## Model selection — not all calls need Opus

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

Matching model to task is a cost and latency decision, not just a capability one.

<!-- incremental_lists: true -->

- **Opus 4.8** — complex reasoning, multi-step synthesis, pricing decisions (you're already using this for AI Pricing)
- **Sonnet 4.6** — most production workloads: replies, summaries, classification, structured extraction
- **Haiku 4.5** — high-volume, low-latency tasks: sentiment tagging, routing decisions, field validation

<!-- incremental_lists: false -->

<!-- column: 1 -->

**Rule of thumb:**

Use the smallest model that gets the job done reliably.

Test with Opus, deploy with Sonnet, consider Haiku for anything running at volume.

<!-- reset_layout -->

<!-- pause -->

Look at two items from your squad's audit wall. For each: which model tier would you actually use, and why? What would need to be true for you to go smaller?

<!-- speaker_note: This question is deliberately economic. Engineers often reach for the most capable model by default. The goal is to start building the habit of model-task matching early. Listen for "we'd need confidence in the output quality" — that's the right instinct, it leads to eval frameworks in Day 2. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 5 — Closing the Loop
===

<!-- end_slide -->

## Back to the support ticket

A guest messages the eviivo Concierge bot: *"The room smells like cigarettes, the shower is broken, and nobody has responded to my last three messages. I'm leaving a review."*

The bot replies: *"Thanks for reaching out! We'll be in touch soon."*

<!-- pause -->

**Now you can answer: who owns this failure?**

<!-- column_layout: [1, 1, 1] -->

<!-- column: 0 -->

**[A] The prompt**

No sentiment detection. No escalation condition. No awareness of prior unanswered messages.

<!-- column: 1 -->

**[B] The workflow**

No trigger to pull booking state. No handoff to a human for high-distress messages. No loop back to check if a reply was sent.

<!-- column: 2 -->

**[C] The missing context**

The model never received: sentiment score, prior message count, property response SLA, guest tier.

<!-- reset_layout -->

<!-- pause -->

The answer is **all three** — and they're connected. A better prompt can't fix missing context. A better workflow can't fix a prompt that has no escalation logic. Context without a workflow to act on it is noise.

<!-- speaker_note: This is the synthesis moment for the morning. You want the team to see that the three failure modes map directly to the three things they'll work on during the course prompts/Skills (Day 1 afternoon), workflows and agents (Day 2), integrations and context pipelines (Day 2–3). Connect explicitly if the room is engaged. -->

<!-- end_slide -->

## Summary

<!-- incremental_lists: true -->

1. **Audit first** — get what exists visible before trying to improve it; invisible work can't be standardised
2. **Variation is the enemy** — different squads solving the same problem differently creates compounding debt
3. **Context is architecture** — what an LLM receives is as important as the prompt that shapes how it uses it
4. **Schema debt is AI debt** — abbreviated field names and undocumented fallbacks directly degrade LLM output quality
5. **Token budget is a design constraint** — not an optimisation for later; it shapes what you retrieve, summarise, and cache
6. **Model selection is a cost decision** — match the model to the task; Opus for reasoning, Haiku for volume

<!-- incremental_lists: false -->

<!-- end_slide -->

## Bridge to the afternoon

**We've established:**

<!-- incremental_lists: true -->

- A shared picture of what your team has built and where practice varies
- The vocabulary for naming problems: prompt debt, context debt, documentation debt
- Why context windows, token budget, and model selection are architectural — not tactical — decisions

<!-- incremental_lists: false -->

**This afternoon:** Skills at Scale — writing, versioning, and sharing Skills that work for more than one person, one squad, and one context.

The audit wall becomes the raw material. Each squad will take one item from their "Dead Ends" lane and rebuild it to shared standards.

<!-- speaker_note: Hard stop here for lunch. Remind the room that the audit board stays up — they'll return to it in the afternoon. If the AI Director has observations from the morning this is a good moment to invite a brief comment (60 seconds max) before breaking. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Questions?
===
