# Web/eCommerce Squad Starter

**Squad:** Web/eCommerce Squad
**Your tool:** `GetGuestConversation(bookingRef)`

## What's already done

- `Program.cs` — host, DI, and MCP server transport are wired and working
- `IBookingApiClient` / `BookingApiClient` — a stub of the unified guest inbox (the API behind Guest Manager — email, WhatsApp, SMS, OTA messaging in one thread). Two booking refs return realistic, hand-written threads:
  - `EVV-2026-00123` — Airbnb guest, simple late-checkout request, resolved
  - `EVV-2026-00456` — Booking.com guest, room-not-as-described complaint, unresolved and getting urgent (useful if your tool needs to demonstrate an escalation-worthy conversation)
  - any other booking ref → a generic but coherent fallback thread, so arbitrary input doesn't error out
- `BookingTools.cs` — the class, attribute, and constructor DI are in place

## What you need to do

Open `BookingTools.cs` and write the `[McpServerTool]` method for `GetGuestConversation`. There's a commented-out example shape in the file — don't just uncomment and fill it in blindly, write your own `[Description]` first.

Worth deciding as a squad: should the description tell Claude this returns the *full* thread (so it can judge sentiment/urgency itself), or should the tool itself flag something like "unresolved" — i.e., is triage the tool's job or the agent's job?

## Run it

```bash
dotnet restore
dotnet build
dotnet run
```

Once your tool compiles and the server starts, use the MCP inspector and call it with both `EVV-2026-00123` and `EVV-2026-00456` to see the contrast between a resolved and an unresolved thread.

## Stretch goal

Add a second tool that would naturally follow — e.g. drafting or sending a reply, or flagging the conversation for escalation.
