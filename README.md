# AI Engineering Intensive

**3-day course** — eviivo R&D Engineering Team, June 2026

---

[FigJam Board](https://www.figma.com/board/w6ThabM3ZV3djApFK6Q8L3/eviivo-AI-Intensive?node-id=0-1&p=f&t=7a4g384w5fLT7VWj-0)

---

## Day 1 — Audit, Alignment, and Agentic Foundations

### Morning: Where Are We Now?
An audit-driven session that surfaces what the team has already built — Skills, prompts, workflows, integrations — and identifies where practice varies across squads. Exercises cover consistency checking, context quality, and establishing shared standards for documentation.

### Afternoon: Agentic Patterns and Skills Done Right
Moves from diagnosis to practice. A structured critique of real Skills using a six-dimension rubric, followed by a lab where delegates rewrite, extend, or author a Skill to a shared standard. Covers tool use, planning loops, multi-step reasoning, and orchestration frameworks.

---

## Day 2 — MCP: From APIs to Architecture

### Morning: MCP Concepts and Internal Design
Builds the conceptual bridge from REST APIs to the Model Context Protocol. Delegates design an internal MCP server — choosing tool names, input/output schemas, and Resources — mapped to a real eviivo API surface. The design sketch becomes the spec for the afternoon lab.

### Afternoon: Building Internal MCP
Hands-on lab: scaffold a working MCP server in C# against a real API surface, wire it to an agent, and compose tools into a workflow. Covers governance, versioning, and the integration pattern with existing Windows services and RabbitMQ infrastructure.

---

## Day 3 — External MCP and Organisational Readiness

### Morning: External MCP and Platform Openness
Shifts focus to the platform boundary — exposing eviivo capabilities to external AI clients and consuming external services safely. Design exercise: either a guest communications MCP composition (sentiment analysis, tone config, workflow triggers) or an ISV partner-facing MCP server (auth, scoping, versioning, data residency).

### Afternoon: Organisational Readiness and the Road Ahead
Steps back from the technical to address team organisation — who owns what, how to govern Skills and MCP servers, review processes, and documentation as a first-class engineering concern. Closes with a structured Q&A, individual learning commitments, and a preview of the autumn follow-up cohort.
