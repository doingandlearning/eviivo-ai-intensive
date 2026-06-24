# Web/eCommerce Squad Starter — Node/TypeScript

Node/TypeScript equivalent of `web-ecommerce-starter/`, for anyone in the lab without
.NET experience. Same exercise, same fake data, same tool to write — different stack.
If you're not blocked on .NET, use the C# starter instead so your squad's output is
consistent.

**Squad:** Web/eCommerce Squad
**Your tool:** `GetGuestConversation(bookingRef)`

## Setup

```bash
npm install
```

## What's already done

- `src/index.ts` — server instance, stdio transport, and connection are wired and working (the equivalent of `Program.cs`)
- `IBookingApiClient` / `BookingApiClient` — a stub of the unified guest inbox (the API behind Guest Manager — email, WhatsApp, SMS, OTA messaging in one thread). Two booking refs return realistic, hand-written threads:
  - `EVV-2026-00123` — Airbnb guest, simple late-checkout request, resolved
  - `EVV-2026-00456` — Booking.com guest, room-not-as-described complaint, unresolved and getting urgent (useful if your tool needs to demonstrate an escalation-worthy conversation)
  - any other booking ref → a generic but coherent fallback thread, so arbitrary input doesn't error out
- `BookingTools.ts` — the registration function and DI-style wiring are in place

## What you need to do

Open `src/BookingTools.ts` and write the `server.tool(...)` call for `get_guest_conversation`. There's a commented-out example shape in the file — don't just uncomment and fill it in blindly, write your own description first.

Worth deciding as a squad: should the description tell Claude this returns the _full_ thread (so it can judge sentiment/urgency itself), or should the tool itself flag something like "unresolved" — i.e., is triage the tool's job or the agent's job?

## Run it

```bash
npm run dev
```

This compiles and starts the server, which then sits waiting on stdio. Use the MCP inspector to confirm the tool is listed, and call it with both `EVV-2026-00123` and `EVV-2026-00456` to see the contrast between a resolved and an unresolved thread.

```bash
npx @modelcontextprotocol/inspector node build/index.js
```

## Stretch goal

Add a second tool that would naturally follow — e.g. drafting or sending a reply, or flagging the conversation for escalation.

## One thing to know about the package

This uses `@modelcontextprotocol/sdk` (v1, currently 1.29.0) — the stable, mature SDK line. There is also a `@modelcontextprotocol/server` v2 package on npm, which is what the _current_ official quickstart docs show but as of this session it's alpha-only (`2.0.0-alpha.2`) and explicitly not API-stable yet. Don't follow the v2-flavoured quickstart examples you might find by searching; this starter is deliberately pinned to v1 so it doesn't break mid-lab. If you want to use v2 later, there's a migration guide linked from the v2 package's README.
