# Day 2 Afternoon — Building Internal MCP

## Session overview

This afternoon you scaffold a real MCP server against your own API surface. By the end of the lab you will have a running server with at least one callable tool, and a clear picture of how it slots into your existing Windows service and RabbitMQ infrastructure.

**Time allocation:**

| Block | Time |
|---|---|
| Scaffold demo + connect to morning | 35 min |
| Governance and workflow patterns | 40 min |
| Lab | 60 min |
| Debrief | 15 min |

---

## Prerequisites

Before the lab starts, make sure you have:

- [ ] .NET 8 SDK installed (`dotnet --version`)
- [ ] VS Code with C# Dev Kit extension
- [ ] Git configured (you'll push your scaffold to a branch)
- [ ] Access to the eviivo internal API documentation (or the stub provided)

---

## Lab exercise

### Your squad and track

| Squad | Members | Starting tool |
|---|---|---|
| Connectivity | Connectivity squad members | `GetChannelSyncStatus(propertyId, channel)` |
| Core Platform | Core Platform squad members | `GetRateAvailability(propertyId, dateRange)` |
| Web/eCommerce | Web/eCommerce squad members | `GetGuestConversation(bookingRef)` |

---

### Step 1 — Create the project

```bash
dotnet new console -n EviivoMcp
cd EviivoMcp
dotnet add package ModelContextProtocol
dotnet add package Microsoft.Extensions.Hosting
```

---

### Step 2 — Wire up the host

Replace `Program.cs` with the minimal host shown in the demo. It should:

- Create an application builder
- Register MCP server services with stdio transport
- Load tools from the assembly
- Start the host

---

### Step 3 — Write your first tool

Create a class for your squad's domain (e.g. `ConnectivityTools.cs`, `RatesTools.cs`, `BookingTools.cs`).

Apply these attributes:

- `[McpServerToolType]` on the class
- `[McpServerTool]` on each method
- `[Description("...")]` on the class, each method, and each parameter

**Write the `[Description]` before writing the implementation.** If you cannot write a clear one-sentence description of when Claude should (and should not) call this tool, stop and discuss it with your squad.

---

### Step 4 — Verify with the MCP inspector

Run `dotnet run` and confirm:

- The server starts without errors
- Your tool appears when you inspect the server
- You can call the tool with a test argument and receive a response

---

### Step 5 (stretch) — Add a second tool or a workflow pattern

Choose one:

**Option A — second tool:** add a complementary tool that Claude would naturally call after the first (e.g. after `GetChannelSyncStatus`, add `TriggerChannelResync`).

**Option B — workflow pattern (Core Platform squad):** sketch a `BackgroundService` that consumes a RabbitMQ message and calls one of your MCP tools. You don't need a live RabbitMQ connection — a stub consumer that logs the call is enough to demonstrate the pattern.

---

### Step 6 — Prepare your debrief

Before the debrief, agree as a squad on:

1. **One decision you made** — a specific architectural or naming decision, not "we chose to start simple"
2. **One thing you'd do differently** — something you'd change if you had another hour, not "we'd have more time"

You will share these with the room. Two minutes per squad.

---

## Reference: the full tool pattern

```csharp
using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerToolType]
[Description("Tools for working with eviivo [your domain] data.")]
public class YourDomainTools
{
    private readonly IYourApiClient _api;

    public YourDomainTools(IYourApiClient api) => _api = api;

    [McpServerTool]
    [Description(
        "One sentence: what this tool does. " +
        "One sentence: when Claude should call it. " +
        "One sentence: any important constraints or caveats.")]
    public async Task<YourReturnType> YourToolName(
        [Description("What this parameter represents and what format it expects.")]
        string yourParameter)
    {
        return await _api.YourMethodAsync(yourParameter);
    }
}
```

---

## Connecting to the session

The workflow pattern from the slides — RabbitMQ message → Windows service → MCP tool call → action — is the same pattern your existing Windows services use, minus the MCP call. You're not replacing anything. You're adding a capability layer between your orchestration logic and Claude.

The guest review scenario from the opening:

```
ReviewReceived (RabbitMQ)
  → ReviewProcessorService (BackgroundService)
    → AnalyseSentiment (MCP tool)
    → CreateSupportTicket (MCP tool, if negative)
```

Your lab builds the MCP tools. The Windows service wrapper is the stretch goal.

---

## Useful commands

| Task | Command |
|---|---|
| Run the server | `dotnet run` |
| Run with hot reload | `dotnet watch run` |
| Build only | `dotnet build` |
| List installed packages | `dotnet list package` |
| Add a NuGet package | `dotnet add package <name>` |

---

## Questions during the lab

If you're blocked for more than 10 minutes, surface it. Don't debug in silence.

Common first blockers:

- **Server won't start** — check that `WithStdioServerTransport()` and `WithToolsFromAssembly()` are both called
- **Tool doesn't appear** — check both `[McpServerToolType]` on the class and `[McpServerTool]` on the method
- **DI not resolving** — register your API client in `builder.Services` before calling `AddMcpServer()`
