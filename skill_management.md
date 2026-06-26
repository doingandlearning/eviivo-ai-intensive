# Skill Discovery at Session Start — and What It Means for Organizing Skills

Claude and GitHub Copilot now implement the same open standard for skills — [Agent Skills](https://agentskills.io) — so the discovery mechanics are nearly identical across both. This matters directly for how an org should structure, name, and govern a shared skill library, and it's the same underlying lesson as the MCP tool description demo (`RatesAndAvailabilityTool` vs. `RatesAndAvailabilityToolVague`): a vague description means the model never picks the right tool, one layer up the stack it means the model never picks the right skill.

## How discovery works at session start

Both ecosystems use a three-layer **progressive disclosure** model:

1. **Discovery layer (always loaded).** When a session starts, every installed skill's `name` and `description` — just the YAML frontmatter, nothing else — is loaded into the system prompt. This happens for *every* installed skill, every session, regardless of whether it's used.
2. **Instructions layer (loaded on trigger).** If the model judges the current request matches a skill's description, it reads that skill's full `SKILL.md` body into context.
3. **Resources layer (loaded on demand).** Any bundled scripts, templates, or reference files are only read or executed if the loaded `SKILL.md` instructs the model to use them. No token cost until that point.

The entire selection mechanism is the model reasoning over `description` text against the user's request. There is no separate routing logic — a skill with a vague or generic description simply won't get triggered, or will get triggered for the wrong requests.

## Skills vs. custom instructions

This distinction determines where an org rule belongs — and it's more nuanced than "all instructions files are always-on":

- **Always-on instructions** — `CLAUDE.md`, `AGENTS.md`, `.github/copilot-instructions.md` — load in full, every session, no discovery step. Use these only for things relevant to nearly all work: coding standards, repo conventions, mandatory rules. In VS Code/Copilot, if more than one of these exists, they all get combined into context together (no guaranteed order) — they aren't alternatives, they stack.
- **`.instructions.md` files are the exception.** These are conditionally applied: matched either against an `applyTo` glob pattern in their frontmatter, or by semantic match between the file's description and the current task. Functionally this is the same progressive-disclosure idea as skills, just scoped to file patterns instead of task descriptions. With no `applyTo` set, a `.instructions.md` file never auto-applies.
- **Skills** load conditionally too, based on description match against the user's request rather than the file being edited. Use these for procedures only some tasks need — a specific workflow, a specialized format, a one-off process.

**Which files a given tool actually reads also varies.** Claude Code reads only `CLAUDE.md` (and `CLAUDE.local.md`) natively — it does not read `AGENTS.md`, `copilot-instructions.md`, or `.instructions.md` at all, unless `CLAUDE.md` explicitly imports `AGENTS.md` via `@AGENTS.md`. VS Code's Copilot Chat, by contrast, auto-detects and combines all three always-on formats (`CLAUDE.md`, `AGENTS.md`, `copilot-instructions.md`) plus any matching `.instructions.md` files. A repo with all four files therefore behaves differently depending on which assistant opens it: Copilot in VS Code loads everything that applies; Claude Code loads only `CLAUDE.md` unless the team has wired the files together.

Putting a narrow procedure into always-on instructions bloats every session's context. Putting a mandatory rule into a skill (or an unscoped `.instructions.md` file) makes it unreliable, since it only fires when the model decides it's relevant.

## Org-level structuring implications

**Description quality is the entire discovery mechanism.** With many skills installed, the model is choosing between them on description text alone — there's no fallback. Anthropic's guidance: write in third person, be specific about *both* what the skill does and *when* to use it, and front-load the key trigger phrase. Treat this with the same rigor as the MCP tool description work already in the `mcp-rates-demo` project.

**Naming conventions differ slightly, and Copilot enforces one.**
- Anthropic recommends gerund-form names (`processing-pdfs`, not `pdf-processor`).
- Copilot/VS Code *requires* the `name` field to exactly match the parent directory name — a mismatch causes a silent load failure, not an error message.

**Context budget is shared and finite.** Claude Code scales the skill-listing budget to roughly 1% of the model's context window. Once an org's skill library exceeds that budget, the least-used skills' descriptions get truncated or dropped first — diagnosable via `/doctor`. At scale, this means actively pruning the library or marking long-tail skills `disable-model-invocation` rather than assuming everything stays visible indefinitely. Description length caps also differ: 1,536 characters in Claude Code vs. 1,024 characters on the Claude Developer Platform API and in VS Code/Copilot.

**Scoping/governance tiers map onto org structure, but the two ecosystems differ in granularity:**

| | Claude Code | Copilot / VS Code |
|---|---|---|
| Org-wide, enforced | Enterprise/managed settings | Org-wide custom instructions (not skill-discovery-based) |
| Personal | `~/.claude/skills/` | `~/.copilot/skills/`, `~/.claude/skills/`, or `~/.agents/skills/` |
| Project (in-repo) | `.claude/skills/` | `.github/skills/`, `.claude/skills/`, or `.agents/skills/` |
| Shared/packaged | Plugin `<plugin>/skills/`, namespaced as `plugin-name:skill-name` to avoid collisions | — |

Claude Code's four-tier model gives finer-grained control (and collision-proofing via namespacing) for an org distributing skills across multiple teams; Copilot's two-tier model is simpler but pushes anything org-wide-and-mandatory into custom instructions instead of skills.

**Security is a real governance concern, not a formality.** A skill is effectively code and instructions the agent will trust once triggered.
- Anthropic recommends auditing any skill from outside your org before installing it — look for anything resembling data-exfiltration instructions.
- GitHub explicitly warns against pre-approving `shell`/`bash` via a skill's `allowed-tools` field unless every script in that skill's directory has been reviewed — doing so removes the human-confirmation gate that normally guards against prompt injection.

For a shared org library, this argues for treating changes to `.claude/skills/` or `.github/skills/` the same way you'd review a new dependency: code review required, especially for anything touching `allowed-tools`.

## Consolidating across formats: pick one source of truth

With four-plus instruction file formats in play (`CLAUDE.md`, `AGENTS.md`, `.github/copilot-instructions.md`, plus tool-specific files like `.cursorrules` and `.windsurfrules`), the 2026 best practice is to stop maintaining separate copies and designate one canonical file instead.

**`AGENTS.md` is the right canonical choice.** It's the cross-vendor standard, now stewarded by the Linux Foundation's Agentic AI Foundation, and read natively by 28+ tools (Copilot, Cursor, Windsurf, Devin, Aider, Zed, JetBrains Junie, and more). The common setup:

- `AGENTS.md` at repo root holds the real content.
- `CLAUDE.md` either symlinks to it, or starts with `@AGENTS.md` (an import) plus any Claude-specific additions below — Claude Code does not read `AGENTS.md` natively, so one of these two is required for Claude Code to see it at all.
- `.github/copilot-instructions.md` symlinks to it, or is reduced to a one-line pointer: "See AGENTS.md."
- Legacy tool-specific files (`.cursorrules`, `.windsurfrules`) get the same one-line-pointer treatment rather than a maintained duplicate.

Two caveats: symlinking `CLAUDE.md` on Windows requires admin rights or Developer Mode, so the `@AGENTS.md` import is the safer cross-platform default. And **Cline does not read `AGENTS.md` yet** — `.clinerules` needs to stay a real file (or a symlink, not just a pointer comment) for teams using Cline.

Don't fold `.instructions.md` (or `.claude/rules/` path-scoped files) into this consolidation — they're conditionally applied to specific file globs by design, not always-on context. Merging them into `AGENTS.md` would just reintroduce the bloat they exist to avoid.

## Where a bash-synced shared skills repo should land

A common setup: an org-wide repo of shared skills, pulled to each developer's machine by a bash script (cron, login hook, dotfiles bootstrap). The sync target should be `~/.claude/skills/<skill-name>/` — one subdirectory per skill, each containing its `SKILL.md`. That's Claude Code's "Personal" tier (applies across every project for that user, not just one repo), and it's also one of the three personal-skill paths VS Code's Copilot reads natively (`~/.copilot/skills/`, `~/.claude/skills/`, `~/.agents/skills/`), so one sync target covers Claude Code and Copilot users alike.

Two trade-offs of doing it this way rather than through an officially supported distribution path:

- **No namespacing at this tier.** Namespacing (`plugin-name:skill-name`) only applies to plugin-distributed skills. A personally created skill with the same directory name as a synced org skill will collide in that same folder.
- **Personal overrides project.** Precedence runs enterprise > personal > project, so anything landing in `~/.claude/skills/` silently wins over a same-named skill checked into a specific project's own `.claude/skills/`.

If collision risk matters at scale, the documented alternative is packaging the shared skills as a Claude Code plugin and distributing through an internal marketplace (`/plugin marketplace add <org-repo>`, `/plugin install <name>@<marketplace>`) instead of a bash sync — plugins get the `plugin-name:skill-name` namespace automatically plus a built-in update mechanism, which is the pattern Anthropic uses for its own first-party skills (e.g. `skill-creator`).

## Practical checklist for structuring an org skill library

- Write descriptions in third person, stating both what the skill does and when to use it, with the key trigger phrase first.
- Match directory name to the `name` field exactly (required by Copilot, good hygiene regardless).
- Use gerund-form names where practical (`writing-skill-md`, not `skill-md-writer`).
- Decide deliberately between a skill (conditional) and a custom-instructions file (always-on) — don't default to instructions just because it's simpler to write.
- Periodically audit the library against the context budget; prune or downgrade rarely-used skills before they get silently truncated.
- Namespace shared/plugin skills to avoid collisions across teams (Claude Code).
- Treat `allowed-tools` (especially shell/bash) as a security review item, not a convenience flag.
- Review any skill sourced from outside the org before installing.

## Sources

- [Equipping agents for the real world with Agent Skills (Anthropic engineering blog)](https://www.anthropic.com/engineering/equipping-agents-for-the-real-world-with-agent-skills)
- [Extend Claude with skills (Claude Code docs)](https://code.claude.com/docs/en/skills)
- [Skill authoring best practices (Claude Developer Platform docs)](https://platform.claude.com/docs/en/agents-and-tools/agent-skills/best-practices)
- [Use Agent Skills in VS Code](https://code.visualstudio.com/docs/agent-customization/agent-skills)
- [Adding agent skills for GitHub Copilot cloud agent (GitHub Docs)](https://docs.github.com/en/copilot/how-tos/copilot-on-github/customize-copilot/customize-cloud-agent/add-skills)
- [Agent Skills open standard](https://agentskills.io)
- [Use custom instructions in VS Code](https://code.visualstudio.com/docs/agent-customization/custom-instructions)
- [How Claude remembers your project — CLAUDE.md and memory (Claude Code docs)](https://code.claude.com/docs/en/memory)
- [Do you symlink your AGENTS.md to other tool-specific files? (SSW.Rules)](https://www.ssw.com.au/rules/symlink-agents-to-claude)
- [Keep your AGENTS.md in sync — one source of truth for AI instructions (Kaushik Gopal)](https://kau.sh/blog/agents-md/)
- [Use Agent Skills in VS Code](https://code.visualstudio.com/docs/agent-customization/agent-skills)
