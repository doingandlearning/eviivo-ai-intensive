# Day 1 Afternoon — Delegate Exercise Sheet

**Session:** Agentic Patterns and Skills Done Right
**Duration:** ~90 minutes (critique + lab)

---

## Part A: Skills Critique (30 minutes)

You and a partner will apply the Skills rubric to real Skills surfaced in this morning's audit.

### The rubric

Score each dimension 1–3:

| Score | Meaning |
|---|---|
| 1 | Absent — this dimension is not addressed at all |
| 2 | Partial — addressed, but incomplete or unclear |
| 3 | Clear — fully specified and usable by someone who wasn't in the room |

### Dimensions

**1. Name clarity**
Does the name tell you what the Skill does and when to use it — without reading the contents? A good name is an action verb + subject + context: `draft-guest-reply`, `escalate-maintenance-complaint`, `calculate-dynamic-rate`.

**2. Input/output definition**
Are inputs named, typed, and marked required/optional? Is the output shape documented — not just "returns something useful"?

**3. Context sufficiency**
Does the system prompt give the model enough to succeed without the engineer sitting next to it? Check: property context, language, persona, relevant business rules, and what the model should NOT do.

**4. Error handling**
What does the Skill specify when a tool call fails? When an input is missing or malformed? When the model is uncertain? Silence is a 1.

**5. Testability**
Can you state one input → expected output pair right now? If not, the Skill cannot be evaluated without running it and hoping. Document at least one test case.

**6. Governance**
Does the Skill have: an owner, a version number, a storage location in the repo, and a review date?

---

### Scorecard

| Dimension | Your Skill | Partner's Skill |
|---|---|---|
| Name clarity | /3 | /3 |
| Input/output definition | /3 | /3 |
| Context sufficiency | /3 | /3 |
| Error handling | /3 | /3 |
| Testability | /3 | /3 |
| Governance | /3 | /3 |
| **Total** | **/18** | **/18** |

---

### Report-back template

Complete this for your pair's highest-impact gap:

**Skill name:**

**The gap (dimension + score):**

**Why this matters in production (one sentence):**

**Minimum change that would raise this score from 1 to 3:**

---

## Part B: Skills Lab (60 minutes)

Pick one of the three options below. Work alone or in a pair.

**Deliverable:** a Skill definition file (markdown or JSON) + a one-paragraph explanation of the decisions you made. Not working code — the Skill specification itself.

---

### Option A: Rewrite a broken Skill

Take a Skill from the morning audit that scored below 10/18 on the rubric.

Rewrite it to the agreed standard. Use the rubric dimensions as your acceptance criteria — your rewrite should score 3 on every dimension.

**Constraints:**
- The Skill must do the same job as the original — don't redesign what it does, only how it is specified
- Document what you changed and why in your one-paragraph explanation

---

### Option B: Extend an existing Skill with error handling and context

Take a Skill that basically works — it produces reasonable output when everything goes right — but fails silently when something goes wrong.

Add:
- Explicit error handling for at least two failure modes (tool call failure, unexpected input, out-of-range value)
- Additional context in the system prompt that the model currently has to guess
- A documented test case that specifically exercises one of the error paths

**Constraints:**
- Do not change the Skill's core function — only its robustness
- Document what your error handling does (doesn't just log and continue — what should the agent do instead?)

---

### Option C: Write a new Skill for an identified gap

Pick one of the gaps identified in the morning session. Suggested starting points:

- **Sentiment analysis trigger:** A Skill that reads an incoming guest message and classifies it (positive / neutral / complaint / urgent complaint) and routes accordingly
- **Check-in workflow step:** A Skill that handles one specific step in the check-in flow — e.g., verifying ID documents, or sending the welcome pack once check-in is confirmed
- **Channel sync event handler:** A Skill that processes an incoming Airbnb/Booking.com event and decides whether to act autonomously or escalate

**Constraints:**
- Define the Skill completely — all six rubric dimensions should score 3
- Be specific about what triggers the Skill and what stops it
- State at least two things this Skill should NOT do (constraints prevent scope creep)

---

### Quality gates before you share

Before presenting, check all four:

- [ ] Can someone use this Skill without asking you any questions?
- [ ] What happens if the first tool call fails? Is that specified?
- [ ] Can you state one input → expected output pair?
- [ ] What version is this, who owns it, and where does it live?

---

### Presentation format (3 minutes per group)

1. What gap did you identify?
2. What decision did you make about the input/output contract?
3. What does your error handling do?
4. What would you need to govern this at team scale?

---

## Reference: Skill definition template

This builds directly on the "minimum viable Skill documentation" template from this morning — same core fields, expanded to match the rubric you just applied:

| This morning's field | Becomes, this afternoon |
|---|---|
| `Name` | `# [skill-name] v[X.Y]` |
| `Owner` | `Owner:` |
| `Purpose` | `## Purpose` |
| `Inputs` | `## Inputs` — now typed, required/optional |
| `Outputs` | `## Expected output` |
| `Failure modes` | `## Error handling` — now one action per condition |
| `Last tested` | `## Test cases` + `Last reviewed:` — now an actual input → output pair, not just a date |

New this afternoon — fields the rubric needs that the morning template didn't ask for: `Trigger`, `## Context`, `## Tools available`, `## Change log`.

```markdown
# [skill-name] v[X.Y]
Owner: [Squad or individual]
Last reviewed: [YYYY-MM-DD]
Trigger: [When is this Skill invoked?]

## Purpose
[One sentence: what does this Skill do, and when should it be used?]

## Context (system prompt additions)
[What the model needs to know: property context, persona, language, 
business rules, constraints, what NOT to do]

## Tools available
- tool_name(param: type, param: type) — description
- tool_name(param: type) — description

## Inputs
- input_name: type — required/optional — description
- input_name: type — required/optional — description

## Expected output
[Shape and meaning of the output. Be specific.]

## Error handling
- If [tool] fails: [what the Skill does]
- If [input] is missing: [what the Skill does]
- If [condition]: [what the Skill does]

## Test cases
| Input | Expected output |
|---|---|
| [example] | [example] |
| [edge case] | [expected] |

## Change log
- v1.0 — [date] — initial version ([author])
- v1.1 — [date] — added error handling for X ([author])
```
