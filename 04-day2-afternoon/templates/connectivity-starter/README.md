# Connectivity Squad Starter

**Squad:** Connectivity Squad
**Your tool:** `GetChannelSyncStatus(propertyId, channel)`

## What's already done

- `Program.cs` — host, DI, and MCP server transport are wired and working
- `IChannelApiClient` / `ChannelApiClient` — a stub of the channel management API. It returns different realistic fake scenarios depending on the `channel` argument:
  - `"Airbnb"` → healthy, synced 4 minutes ago
  - `"Booking.com"` → sync error (a rejected rate plan mapping, with a real-looking error message)
  - `"Expedia"` → pending updates, synced 47 minutes ago
  - `"Vrbo"` → healthy, synced 12 minutes ago
  - anything else → a generic "pending updates" response, so unrecognised channel names don't blow up
- `ConnectivityTools.cs` — the class, attribute, and constructor DI are in place

## What you need to do

Open `ConnectivityTools.cs` and write the `[McpServerTool]` method for `GetChannelSyncStatus`. There's a commented-out example shape in the file — don't just uncomment and fill it in blindly, write your own `[Description]` first.

Questions worth settling as a squad before you write the description:
- When should Claude call this tool versus not call it? (e.g. is this for "is X synced right now" questions, or also for "is anything broken across all channels")
- Does `channel` need format guidance in its `[Description]` (exact casing, the four channel names, what happens with an unrecognised one)?

## Run it

```bash
dotnet restore
dotnet build
dotnet run
```

Once your tool compiles and the server starts, use the MCP inspector to confirm the tool is listed and call it with `channel = "Booking.com"` to see the error scenario come back.

## Stretch goal

Add `TriggerChannelResync(propertyId, channel)` as a second tool — the natural next action after spotting an error or pending-updates state.
