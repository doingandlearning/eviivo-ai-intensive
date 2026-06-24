# Facilitator Guide — Day 2 Afternoon: Building Internal MCP

## Session at a glance

**Audience:** 15 engineers and managers at eviivo (hospitality SaaS). Onsite, Islington. Afternoon of Day 2.
**Room energy:** Post-lunch, post-morning design session. Expect some fatigue. The hands-on lab is what rescues the afternoon — get to code fast.
**Tone:** Practitioner to practitioner. Not a lecture. You build, they watch, then they build.

---

## Pre-session checklist

- [ ] VS Code open, `EviivoMcp` project pre-created but not yet run (you'll run it live)
- [ ] MCP inspector open in a second terminal pane
- [ ] `claude_desktop_config.json` (or VS Code MCP settings) staged but not yet pointed at the server
- [ ] eviivo internal API stub available (or confirmed with the Connectivity lead that real endpoints are accessible on the network)
- [ ] RabbitMQ connection string for the BackgroundService demo — even a local dev instance is fine
- [ ] Confirmed with the DevOps engineer: does eviivo's Windows service deployment process affect how they'd host this? Know the answer before the session.
- [ ] the AI Director briefed: he's Day 3 of his employment, 13 years AI experience. He's NOT the target learner. He can add colour when asked; don't let him take over.

---

## Block-by-block notes

### Block 1: Quick connect to morning (~15 min)

**Goal:** make the bridge from the morning design exercise explicit without re-teaching anything.

Ask the room: "What's the most interesting tool you designed this morning?" Take two or three answers. The goal is to surface real eviivo domain language — rates, availability, bookings, channel sync — so the demo feels like their problem, not a generic example.

**Don't recap the morning's slides.** If someone missed the morning session, they'll catch up in the lab. Don't slow the room for one person.

---

### Block 2: Scaffold demo (~20 min)

**Run everything live.** The NuGet install noise, the first compile error if one happens, the MCP inspector connection attempt — all of it. A clean pre-prepared demo is worse than a slightly messy live one.

**Key moments to narrate:**

1. `dotnet add package ModelContextProtocol` — "This is the official Anthropic SDK for .NET. One package. No framework. It graduated out of prerelease a while back, so no `--prerelease` flag needed any more."

2. `WithStdioServerTransport()` — "Stdio is how Claude Desktop and VS Code talk to MCP servers locally. You're not running a web server. You're running a process that Claude talks to over stdin/stdout. For internal tools on a Windows service host, that's almost always what you want."

3. The `[Description]` attribute — "This attribute is the entire contract between your code and Claude. It is the documentation. It is the schema. If this is vague, Claude will use the tool incorrectly. We'll come back to this in the governance section."

4. MCP inspector call — after a successful tool call, ask the room: "What did Claude just see?" Let them answer. The answer is: a JSON schema derived entirely from your C# attributes.

**If the demo breaks:** debug it live. Say: "This is what day one looks like. Let's fix it." It's more valuable than a clean demo.

---

### Block 3: Governance and team ownership (~20 min)

**The ownership table is a live decision for this room.** Don't present it as received wisdom.

After showing the table (1–3 / 4–10 / platform team), say: "For eviivo right now, I'd argue each squad owns their server and the DevOps engineer owns the deployment pipeline. Disagree with me."

**Anticipated responses:**

- "What about shared tools?" — Good. If two squads need `GetBookingStatus`, who owns it? Answer: the squad closest to the booking domain (Web/eCommerce). Others take a dependency. This becomes a platform conversation when there are more than three shared tools.
- "How do we version the descriptions?" — Treat `[Description]` changes like API breaking changes. Semver the server. Keep a CHANGELOG.
- "What about the NuGet package version vs. the tool schema version?" — Separate concerns. The NuGet package version tracks the SDK. The server version tracks your tool schema. They're independent.

**The schema debt connection:** explicitly link this to whatever came up in Day 1 about context debt. If Day 1 used the phrase "context debt," use it here. "A vague `[Description]` attribute is context debt. You're borrowing against future reliability."

**Call on the AI Director for perspective here if the room is quiet.** He has strong opinions on schema design. One sentence from him can unlock the room.

---

### Block 4: Workflow automation patterns (~20 min)

**This section belongs to the DevOps engineer and the back-end architect.** Before you start, say: "The DevOps engineer, the back-end architect — this is your section. I'll show the pattern, but you know where the Windows services actually live."

**The RabbitMQ diagram** — walk through it top to bottom. The key insight to land: "The MCP server doesn't know about RabbitMQ. The Windows service doesn't know about Claude. The boundary is clean by design." If they push back — "why not just call Claude directly from the Windows service?" — the answer is: you can. But the MCP server gives you testability, reusability across multiple services, and a versioned schema.

**The `BackgroundService` code** — don't read it aloud. Give them 30 seconds to read it, then ask: "What would you change for your squad's scenario?" The Connectivity squad should spot that their consumer would fire on a channel sync event, not a review event. Core Platform should see the rates polling pattern. Web/eCommerce should see the guest review path directly.

**Anticipated questions:**

- "Can a Windows service host an MCP server over HTTP instead of stdio?" — Yes. The SDK supports Streamable HTTP transport. But start with stdio locally. Streamable HTTP is a Day 3 conversation.
- "What's the latency of a Claude call in a Windows service?" — It depends on model and context size. For triggered workflows, latency is usually acceptable. For real-time pricing (the senior full-stack engineer, a senior engineer), it's a design constraint — you'd cache or pre-compute.
- "Do we need a separate process for the MCP server?" — For the lab, no. In production, yes — the MCP server is a separate deployable. the DevOps engineer should weigh in here.

---

### Block 5: Skills + MCP composition (~15 min)

**This is the payoff.** The whole afternoon has been building to "here's how the pieces fit together."

When you show the full chain diagram, don't rush past it. Say: "Read this. Tell me what's new here versus what you already have." What's new is the MCP tool call in the middle. Everything else — the RabbitMQ message, the Windows service, the internal API — is already there.

**The closing provocation return** — "How many manual steps sit between 'review posted' and 'ticket opened'?" was the opening question. After showing the chain, say: "Zero. That's the answer." Then move to the lab.

---

### Block 6: Lab (~60 min)

**Your role during the lab:** circulate, don't lecture.

**30-minute checkpoint:** ask each squad: "What's your working tool?" If any squad doesn't have a tool that calls successfully in the MCP inspector, help them unblock before the stretch goal.

**Common blockers and fixes:**

| Blocker | Fix |
|---|---|
| Server won't start | `WithStdioServerTransport()` missing, or `WithToolsFromAssembly()` missing |
| Tool doesn't appear | Missing `[McpServerToolType]` on class OR missing `[McpServerTool]` on method — need both |
| DI not resolving | API client not registered in `builder.Services` before `AddMcpServer()` |
| MCP inspector can't connect | Wrong path in config, or `dotnet run` hasn't started yet |
| `dotnet restore` fails with a version error | The package is stable now — no `--prerelease` flag needed. Check nuget.org for the actual highest published version of `ModelContextProtocol`; a `<PackageReference>` floor above what's published will fail restore |

**Squad-specific notes:**

- **Connectivity squad (the Connectivity lead, the Connectivity architect, a Connectivity engineer):** They'll hit auth questions first. Remind them: stdio transport + service account = their auth boundary. The MCP server runs as a Windows service identity. If they want to add explicit auth later, that's a header injection on the HTTP client, not an MCP protocol concern.

- **Core Platform squad (the Core Platform lead, the back-end architect, a senior engineer):** the back-end architect will want to go deep on the RabbitMQ pattern. That's fine — he's the right person for it. Keep a senior engineer focused on the tool schema for rates/availability. The stretch goal (BackgroundService sketch) is realistic if they get the tool working in the first 30 minutes.

- **Web/eCommerce squad (the eCommerce lead, a front-end engineer, an engineer, an engineer, an engineer):** This squad is likely to go too broad too fast. the eCommerce lead has big ideas about the booking engine; a front-end engineer will want to architect it properly. Keep them scoped to one end-to-end chain. "GetGuestConversation → stub sentiment → return result" is enough. The architecture discussion can happen in the debrief.

**the front-end manager and the senior full-stack engineer:** the front-end manager may float between squads — that's fine. the senior full-stack engineer may end up helping the Core Platform squad on data aggregation; let him. He's useful there.

---

### Block 7: Debrief (~15 min)

**Go in order:** Connectivity → Core Platform → Web/eCommerce.

**The frame:** "One decision you made. One thing you'd do differently. Not a show-and-tell — we want the hard calls."

**If squads give safe answers** ("we decided to start simple"), push: "What did you argue about? What did you cut?" The goal is to surface real architectural disagreements — not polished summaries.

**Good debrief answers that have come up in similar sessions:**

- "We argued about whether one server should own both rates and availability, or whether they should be separate"
- "We realised our API client has no stable contract — the tool description was harder to write than the implementation"
- "We cut the BackgroundService stretch because we couldn't agree on the message schema"

**the AI Director:** if he offers perspective during the debrief, give him the floor briefly. His 13-year AI background is credible here, especially on architecture. One minute max — the room is tired.

---

## Bridge to Day 3

The Day 3 morning session is **External MCP and Platform Openness**. The ownership and versioning decisions made today become significantly more consequential when external clients can call your tools.

End the session with: "Tomorrow morning we take these same servers and ask: what changes when the Connectivity lead's Connectivity MCP is callable by Booking.com or Expedia? The governance model you designed today is the starting point."

---

## Anticipated off-topic questions

**"Can we use Python instead of C#?"**
Yes. The ModelContextProtocol SDK exists for Python. But the session is Windows/VS Code-first because that's your stack. C# is the right call for eviivo. If someone wants to explore Python after hours, great.

**"What about MCP over HTTP for our existing REST services?"**
Streamable HTTP transport supports HTTP. It's a Day 3 conversation — external MCP needs HTTP. For internal use, stdio is simpler and more secure.

**"Can Claude call multiple MCP servers in one conversation?"**
Yes. That's the composability story. A Skill can call tools from the Connectivity MCP, the Rates MCP, and the Booking MCP in the same conversation. The user never knows which server answered.

**"What about cost? Each MCP tool call = a Claude API call?"**
Not exactly. MCP tool calls happen within a conversation context. You pay for the tokens in + tokens out for the whole exchange, not per tool call. Caching helps significantly for repeated contexts.

**"Can we test MCP tools without Claude?"**
Yes — use the MCP inspector directly, or write unit tests that call the tool methods directly (they're just C# methods). The MCP protocol is the transport; the logic is testable in isolation.

---

## Energy management

- **Post-lunch slump:** the scaffold demo is the antidote. If the room is flat at the start, skip the agenda slide and go straight to `dotnet new console`. Code is more engaging than a table.
- **Lab energy:** circulate actively. Squads that go quiet are usually stuck, not productive.
- **Debrief energy:** keep it tight. 15 minutes max. The room needs to finish energised for Day 3 morning.
