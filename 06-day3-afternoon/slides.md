---
title: "**Organisational Readiness and the Road Ahead**"
sub_title: AI Engineering Intensive — Day 3 Afternoon
author: Kevin Cunningham
---

## A question for the room

It's 90 days from now. the AI Director has been in post three months.
The team has shipped two AI features. One is celebrated. One quietly failed.

**What was different about the two?**


<!-- speaker_note: Give 60 seconds. Don't resolve the tension — let it sit. The point is that "AI quality" is not just about models. We will return to this at the close. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 1 — How Do We Organise Around This?
===

<!-- end_slide -->

## Three models. One decision.

<!-- column_layout: [1, 1, 1] -->

<!-- column: 0 -->

**Centralised AI team**

A dedicated group owns all AI work. Squads submit requests.

- Fast to start
- Quality ceiling rises quickly
- Bottleneck by month 6
- Squads stay dependent

<!-- pause -->

<!-- column: 1 -->

**Embedded AI in squads**

Each squad owns its AI capability. No central function.

- Moves fast per squad
- Knowledge fragments
- No shared standards
- Hard to maintain consistency
<!-- pause -->
<!-- column: 2 -->

**Guild model**

Embedded AI in squads + a guild that sets standards, reviews work, shares patterns.

- Slower to spin up
- Compounds over time
- Needs a connector
- **This is what you've been building**

<!-- reset_layout -->

<!-- speaker_note: The guild model is the answer for eviivo right now — but don't tell them that yet. Surface the question first. -->

<!-- end_slide -->

## The connector role

A guild only works if someone holds the connective tissue.

<!-- pause -->

That person:

<!-- incremental_lists: true -->

- Sees across squads, not just into them
- Translates between engineering capability and product opportunity
- Sets the bar for what "good AI work" looks like
- Reviews Skills, prompts, and agent designs before they ship
- Runs the retrospective loop — what worked, what didn't, why

<!-- incremental_lists: false -->


<!-- speaker_note: This is the moment to explicitly turn to the AI Director. "You've been watching all week. What would you add to that list? What would you push back on?" Give him the floor for 3–4 minutes. This is the baton pass. -->

<!-- end_slide -->

## What does the guild actually do?


> What do you want from a guild that you don't currently have?

Bring back one sentence per squad lead.

<!-- speaker_note: Pre-assign the Connectivity lead, Core Platform lead, eCommerce lead. Three answers = three different pressures on the guild. Record them. These become the guild's founding charter. the AI Director should hear them as mandate, not constraint. -->

<!-- end_slide -->

## Governance without gatekeeping

The risk: governance becomes a queue. Someone submits a Skill. It waits two weeks for review.

<!-- pause -->

**What you might want instead:**

<!-- column_layout: [3, 2] -->

<!-- column: 0 -->

<!-- incremental_lists: true -->

- A shared Skill registry with clear ownership- A lightweight review checklist — not an approval board
- A `[Description]` standard that makes intent reviewable at a glance
- A "graduated autonomy" model — new engineers get reviewed, experienced engineers self-certify

<!-- incremental_lists: false -->

<!-- column: 1 -->

**The question is not:**
"Did someone approve this?"

**The question is:**
"Would the team be proud of this if a customer saw the output?"

<!-- reset_layout -->

<!-- speaker_note: The `[Description]` attribute pattern was introduced in Day 2 MCP work. Connect it here explicitly. Governance lives in the artefacts, not in a committee. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 2 — Documentation as Infrastructure
===

<!-- end_slide -->

## The team that writes the best documentation gets the most capable AI

Not metaphorically. Literally.

<!-- pause -->

Three things the AI reads that you probably under-invest in:

<!-- incremental_lists: true -->

- **Schema field naming** — `prop_val_3` teaches the model nothing. `base_rate_gbp` teaches it everything.
- **Skill `[Description]` attributes** — the routing decision for every agentic call lives here
- **Context documentation for agents** — the system prompt is not a place to put your architecture

<!-- incremental_lists: false -->

<!-- pause -->

The platform's schema debt isn't just a DX problem. It's an AI capability ceiling.

<!-- speaker_note: This closes the loop from Day 1's schema debt conversation. a senior engineer and the back-end architect will feel this most directly. Invite them to respond. -->

<!-- end_slide -->

## Decide today: the Skill registry

You talked about this on Day 1. Now it needs a decision.

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

<!-- incremental_lists: true -->

**Minimum viable registry entry:**

- Skill name and version
- Owner (squad + engineer)
- `[Description]` — what it does, in one sentence, as the AI sees it
- Inputs and outputs typed
- Last reviewed date

<!-- column: 1 -->

**Where does it live?**

**[GitHub repo]** / **[internal wiki]** / **[Confluence]** / **[JIRA]** / **[MCP Prompts]**

**Who owns the registry process?**

<!-- reset_layout -->

<!-- speaker_note: Push for a named owner before leaving this slide. "We'll figure it out later" means it doesn't happen. If nobody volunteers, suggest the AI Director as registry steward with a squad rotation for reviews. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 3 — AI-Assisted Development in a Microsoft Shop
===

<!-- end_slide -->

## Which tool for which job

You have: GitHub Copilot, Claude in VS Code, Claude Code/Cowork, and the MCP servers you just built.

<!-- column_layout: [2, 3] -->

<!-- column: 0 -->

**The trap:**

Treating these as competing products and picking one.

<!-- pause -->

**The reality:**

They're different tools for different jobs — like a linter and a code review aren't competing.

<!-- column: 1 -->

| Job | Tool |
|---|---|
| Autocomplete while typing | GitHub Copilot |
| Chat about the file I'm in | Claude in VS Code |
| Multi-file refactor, planning, review | Claude Code |
| Query our own platform data | MCP servers (what you built) |
| Orchestrated agent workflows | Claude + MCP + Skills |

<!-- reset_layout -->

<!-- speaker_note: a front-end engineer and the front-end manager will likely ask about the JS/React experience specifically. The VS Code Claude extension is good here. The DevOps engineer will want to know about CI/CD integration — Claude Code has a headless mode worth mentioning. -->

<!-- end_slide -->

## What does a senior engineer's day look like now?

A senior engineer's question, answered honestly.

<!-- pause -->

<!-- incremental_lists: true -->

- Morning: Copilot handles the boilerplate. You spend less time on the obvious code.
- Review: Claude Code does a first pass — catches the things you'd catch in review, before review.
- Design: You describe the intent, the agent drafts the Skill or MCP handler. You assess and adjust.
- Hard problems: still yours. The model is a fast first draft, not a decision-maker.
- End of day: you've shipped more, reviewed more, and documented more — because the tooling made documentation cheap.

<!-- incremental_lists: false -->

<!-- pause -->

**The skill that compounds: knowing when to trust the output and when to push back.**

<!-- speaker_note: the senior full-stack engineer may have a strong view here. Invite them "Does that match what you've seen this week?" This is also the honest answer for the Head of Engineering if they're on the call. -->

<!-- end_slide -->

## The DevOps engineer's question: operationalising this

AI-built code ships through the same pipeline as everything else. Or it should.

**Does your CI pipeline currently treat AI-assisted code any differently?**

<!-- pause -->

Three things worth deciding:

<!-- incremental_lists: true -->

- Do AI-generated Skills get reviewed before merge, or just tested?
- Does the MCP server you built this week have an owner? A deployment slot?
- When an agent produces output that causes a customer issue — what's the incident process?

<!-- incremental_lists: false -->

<!-- speaker_note: The DevOps engineer should carry these action items. Don't solve them now — surface them as the right questions for the next sprint planning. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 4 — Learning Without Getting Overwhelmed
===

<!-- end_slide -->

## One thing, by role

You have full Coursera access. That's both a gift and a trap.

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**Squad leads (Connectivity, Core Platform, eCommerce, Connectivity)**
→ [AI For Everyone](https://www.coursera.org/learn/ai-for-everyone) — the course instructor. 6 hours. Builds the vocabulary to manage AI work without needing to implement it.

**Senior engineers**
→ [Generative AI with LLMs](https://www.coursera.org/learn/generative-ai-with-llms) — DeepLearning.AI. Grounds the model behaviour you've been using this week in how it actually works.

<!-- column: 1 -->

**Managers**
→ [AI Product Management](https://www.coursera.org/specializations/ai-product-management-duke) — Duke. 3 courses. Connects AI capability to product decisions.

**Everyone**
→ [Anthropic Academy](https://www.anthropic.com/education) — prompt engineering and API courses. Free. Directly relevant to the Skills pattern you've been building.

<!-- reset_layout -->

<!-- speaker_note: Don't read all of these aloud. Put the slide up, give people 90 seconds to note the one relevant to them. Then move. The point is one thing, not everything. -->

<!-- end_slide -->

## Staying current without getting overwhelmed

The field moves fast. Most of the noise is not relevant to your stack.

<!-- pause -->

**Signal filters that I've seen work:**

<!-- incremental_lists: true -->

- Follow Anthropic's release notes — they are brief and technically specific
- One Slack channel for AI engineering news — one person curates, others read
- The AI Director's role includes surfacing what's relevant to eviivo specifically
- A quarterly "what's changed" review: 30 minutes, the AI Director facilitates, squads bring observations

<!-- incremental_lists: false -->

<!-- pause -->

**What to ignore:** benchmarks, launch announcements, and "GPT-5 vs Claude" discourse. Watch what ships in production at companies like yours.

<!-- speaker_note: This is a light slide. Move through it quickly. The point is "you don't have to read everything — here's who filters it." -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 5 — Real Problems from the Platform
===

<!-- end_slide -->

## Your problems, not hypotheticals

Each squad lead: one real problem on the table. Two minutes each.

Not "we should probably think about AI for X" — a specific thing that is hard right now.



<!-- speaker_note: The facilitator leads. The AI Director contributes — this is where they start being the practitioner, not the observer. If the Head of Engineering is on video, they get the floor after the three squad leads "Given what you've heard, what do you want to see shipped in 90 days?" Write the three problems on a shared screen. This is the input to the open session. -->

<!-- end_slide -->

## Open session

The three problems are on the table. Forty-five minutes.

<!-- pause -->

**How this works:**

<!-- incremental_lists: true -->

- One problem at a time
- Five minutes of applied thinking from the room
- One owner, one action, one date — before moving to the next problem

<!-- incremental_lists: false -->

<!-- speaker_note: This is the least-structured block of the week. That is intentional. The discipline is in the format owner + action + date before moving on. Don't let it become a discussion without outputs. Record the three decisions. They go into the README the delegates take away. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 6 — Autumn and What Comes Next
===

<!-- end_slide -->

## What Track 2 builds on

The June foundations apply whether the Autumn cohort is the same 15 or the next 15.

<!-- column_layout: [1, 1] -->

<!-- column: 0 -->

**What you built in June:**

<!-- incremental_lists: true -->

- A shared vocabulary for Skills, prompts, and context
- An MCP pattern for connecting AI to your platform
- A guild model with a named connector
- A Skill registry/MCP with an owner
- Three real problems with owners and dates

<!-- incremental_lists: false -->

<!-- column: 1 -->

**What Track 2 builds on top:**

<!-- incremental_lists: true -->

- ML fundamentals — how models actually work, not just how to prompt them
- Open source models — when to run your own, when to call the API
- The pricing model — a senior full-stack engineer's work, grounded in the ML layer
- Fine-tuning on domain data — eviivo's schema as training signal

<!-- incremental_lists: false -->

<!-- reset_layout -->

<!-- speaker_note: Keep this brief. The point is the work this week has lasting value. It is not a standalone event. The Autumn cohort — whether same group or second 15 — inherits everything built here. -->

<!-- end_slide -->

<!-- jump_to_middle -->

Part 7 — Close
===

<!-- end_slide -->

## Three days as principles

Not topics. Principles you can use Monday morning.

<!-- incremental_lists: true -->

1. **Context is the capability.** The model is only as good as what you give it. Schema names, Skill descriptions, agent context — this is the engineering work that multiplies everything else.

2. **The architecture is the interface.** MCP servers, Skill registries, governance processes — these are not admin overhead. They are the product.

3. **Capability compounds with ownership.** A guild with no connector decays. A connector with no guild has no leverage. You now have both.

<!-- incremental_lists: false -->

<!-- end_slide -->

## Back to the opening question

90 days from now. One feature celebrated. One quietly failed.

**What was different?**

<!-- pause -->

The celebrated one had:

<!-- incremental_lists: true -->

- Clear ownership (the guild connector knew what was in flight)
- A Skill/Prompt/Tool with a `[Description]` that the team could read and trust
- A schema that the model could navigate
- A review process that asked "would we be proud of this output?" not "did someone approve it?"

<!-- incremental_lists: false -->

<!-- pause -->

The failed one had none of that. The model was fine. The infrastructure wasn't ready.

<!-- pause -->

**You now know how to build the infrastructure. The rest is execution.**

<!-- speaker_note: Return to the opening chat poll results. Read back what people typed. "You said people, process, infrastructure. You were all right — but today you built a plan for all three." Close here. No further slides. -->

<!-- end_slide -->
