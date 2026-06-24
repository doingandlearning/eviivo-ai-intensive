# Day 2 PM Starter Templates

Three pre-built .NET 8 console projects, one per squad. Each one builds and runs out of the box with zero MCP tools registered — the squad's lab task is to add the tool described in their README, not to scaffold the project from scratch.

| Folder | Squad | Members | Tool to write |
|---|---|---|---|
| `connectivity-starter/` | Connectivity | Connectivity squad | `GetChannelSyncStatus(propertyId, channel)` |
| `core-platform-starter/` | Core Platform | Core Platform squad | `GetRateAvailability(propertyId, dateRange)` |
| `web-ecommerce-starter/` | Web/eCommerce | Web/eCommerce squad | `GetGuestConversation(bookingRef)` |

## What's pre-built in every starter

- `Program.cs` — host builder, DI registration, `AddMcpServer().WithStdioServerTransport().WithToolsFromAssembly()`, all wired correctly
- A stub API client (interface + implementation) returning realistic fake data for the squad's domain — no real eviivo API access needed
- A `Tools.cs` file with the class skeleton, `[McpServerToolType]` attribute, and constructor DI already in place, plus a commented-out example showing the exact pattern
- A squad-specific `README.md` with the one task: write the `[McpServerTool]` method and its `[Description]`

Nothing about the actual tool — method body, attributes, description text — is filled in. That's the lab's point: writing the `[Description]` before the implementation, per the existing Day 2 PM README.

## Before the session — one thing to check

Each `.csproj` pins `ModelContextProtocol` at version `1.3.0` — the highest version actually published for that package (verified directly against the live NuGet feed, June 2026). The package is out of prerelease, so the `--prerelease` flag in the original lab Step 1 instructions is no longer needed for anyone scaffolding fresh.

**Don't bump this to 1.4.0 without checking first.** `ModelContextProtocol.AspNetCore` does have a 1.4.0 release, but the base `ModelContextProtocol` package (what these stdio console starters use) does not — its dependency graph tops out at 1.3.0. An earlier draft of these starters pinned 1.4.0 for the base package; that version doesn't exist, and `dotnet restore` fails outright with an unresolvable version floor. This has been corrected.

Run a `dotnet restore` inside each starter folder once, on a machine with normal network access, before the session. If the package has moved on by the 24th, bump the `<PackageReference Version="...">` line — just confirm on nuget.org that the version you're bumping to actually exists for `ModelContextProtocol` specifically, not just for the AspNetCore companion package.

## No .NET experience? Node/TypeScript alternative

`web-ecommerce-starter-node/` is a Node/TypeScript port of `web-ecommerce-starter/` — same
exercise, same fake data, same tool signature (`GetGuestConversation(bookingRef)`), built on
the official `@modelcontextprotocol/sdk` (stable v1, pinned to 1.29.0). Hand this to anyone
who'd otherwise be blocked by the C# requirement, regardless of which squad they're actually
on — the domain doesn't need to match their squad for the exercise to land.

Verified June 2026: `npm install && npm run build` compiles clean, `node build/index.js` starts
and sits waiting on stdio with no errors. Don't let anyone follow the current official
quickstart docs verbatim — those now document the v2 package (`@modelcontextprotocol/server`),
which is alpha-only and not API-stable. This starter is deliberately pinned to v1.

## How to distribute

Copy each squad's folder to that squad's machine (or zip and share), or have them `git clone`/pull if you push this into the course repo. Each folder is self-contained — no shared project references between squads.

## Verifying a starter works before the session

```bash
cd connectivity-starter
dotnet restore
dotnet build
dotnet run
```

It should start and sit waiting on stdio with no errors and no tools listed (since none are written yet). Ctrl+C to stop. Repeat for the other two folders.
