# Day 1 Morning — Delegate Exercise Sheet

## Session: Where Are We Now?

**eviivo AI Engineering Intensive — Day 1, 24 June 2026**

---

## The Audit Activity

This is the core work of the morning. Before any frameworks or theory, we need to get what your team has already built out of people's heads and onto the wall.

**Time: 15 minutes (parallel across squads)**

---

### What to Bring to the Wall

Each person contributes **at least four stickies** — one per category below. Write one item per sticky. Be specific: a vague sticky ("we use AI sometimes") is not useful.

#### Category 1: Skills and Prompts

Anything you've written that shapes how an LLM responds — a system prompt, a Claude Skill, a reusable prompt template, a GitHub Copilot instruction file.

For each, capture:
- What it's called (or what you call it informally)
- What it does in one sentence
- Where it lives (in a repo? in someone's notes? in a shared doc? nowhere?)
- Who else on your squad knows it exists

#### Category 2: AI Workflows

Any process that involves an LLM at one or more steps — even if the LLM step is small or informal. This includes things you've prototyped but not shipped.

For each, capture:
- What the workflow does (input → process → output)
- Which LLM call(s) are involved
- What triggers it and what it produces
- Whether it's in production, in development, or abandoned

#### Category 3: Integrations

Any connection between eviivo systems and an AI model or AI-enabled tool. This includes: API calls to Anthropic or OpenAI, third-party tools that use AI internally, Claude integrations via GitHub or VS Code.

For each, capture:
- What system calls what
- What context is passed to the model (what fields, what data)
- What the model returns and how it's used
- Whether it's documented

#### Category 4: Dead Ends

Things you tried that didn't work — experiments abandoned, prompts that kept failing, integrations that were too unreliable to ship.

For each, capture:
- What you tried
- What happened
- Your best current guess at why it didn't work
- Whether you'd try again with more knowledge

---

### How to Sort What's on the Wall

After the 15-minute contribution phase, your squad will spend 5 minutes sorting and tagging.

Apply one or more tags to each sticky:

| Tag | Meaning |
|---|---|
| `VISIBLE` | Anyone on the squad can find and use this |
| `SILO` | Only one person knows this exists or how it works |
| `UNDOCUMENTED` | Works but has no written explanation |
| `ASSUMED` | Relies on context that isn't written down anywhere |
| `FRAGILE` | Would break if a specific person left, or if a field changed |
| `DUPLICATE` | Another squad has probably solved the same problem |

---

### Squad Debrief Questions

After sorting, each squad nominates one spokesperson to report back:

1. **What surprised you?** — something on the wall you didn't know existed, or a gap you didn't expect
2. **What's your most fragile item?** — the thing that would break most badly if one person left or one assumption changed
3. **What's your best work?** — the item you're most proud of, and what makes it better than the others

---

## Part 2: The Consistency Exercise

After the full-group debrief, you'll compare approaches across squads.

### Exercise: The Same Task, Different Squads

Pick one task that more than one squad handles — for example: **writing a reply to a guest message**.

**Step 1** — Each squad member writes down (individually, not collaborating):
- The prompt or Skill they would use for this task
- The context they would include
- How they would handle a situation where the guest is clearly upset

**Step 2** — Share within your squad. Identify:
- What choices are consistent across your squad
- What choices vary (even slightly)
- What assumptions everyone made without writing down

**Step 3** — Bring one specific difference to the full group:
> "Our squad disagreed about whether to include ___________. Some of us assumed _________, others assumed _________."

---

## Part 3: Context Audit

After the consistency exercise, pick **one integration or workflow** from your audit wall — ideally one that's in production or nearly there.

Answer these questions about the context that LLM receives:

| Question | Your answer |
|---|---|
| What fields are passed to the model? | |
| Are any fields abbreviated or using internal codes? | |
| What happens if a field is null or empty? | |
| What happens if a field contains unexpected values? | |
| Where is the context constructed? (which service/layer?) | |
| If the model produced a wrong answer last week, where would you look first? | |

---

## Reference: What Good Documentation Looks Like

You don't need a sophisticated system. You need enough that someone new (or you, in six months) can understand what a Skill does and how to use it safely.

**Minimum viable Skill documentation:**

```
Name: [verb-noun format, e.g. reply-guest-message]
Owner: [squad name]
Purpose: [one sentence — what problem does this solve?]
Inputs: [list every field the model receives — name, type, required/optional, fallback]
Outputs: [what does a successful response look like? what does a failure look like?]
Failure modes: [what inputs cause bad outputs? what should happen instead?]
Last tested: [date]
```

You don't need to fill this in now. In the afternoon session, each squad will write one entry for a Skill from their audit wall.

---

## Notes Space

Use this space during the morning to capture:

- Things from other squads' walls that surprised you
- Questions you want to ask in the debrief
- Assumptions in your own work you hadn't thought about before
- Ideas for the afternoon session

```
Your notes:




```
