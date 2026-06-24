# Core Platform Squad Starter

**Squad:** Core Platform Squad
**Your tool:** `GetRateAvailability(propertyId, dateRange)`

## What's already done

- `Program.cs` — host, DI, and MCP server transport are wired and working
- `IRatesApiClient` / `RatesApiClient` — a stub of the rates/availability gateway. Given a property and a date range, it generates one entry per night with:
  - `BaseRateGbp` — weekday vs weekend pricing (weekends cost more)
  - `RoomsAvailable` — tightens towards weekends
  - `HasStopSellActive` — true on an occasional weekend night, with `RoomsAvailable` forced to 0 on that night (a stop-sell night should never be sellable even if the raw count says otherwise)
- `RatesTools.cs` — the class, attribute, and constructor DI are in place

## What you need to do

Open `RatesTools.cs` and write the `[McpServerTool]` method for `GetRateAvailability`. There's a commented-out example shape in the file.

The lab brief leaves `dateRange` deliberately vague — that's your squad's design decision. The stub takes `DateOnly checkIn, DateOnly checkOut`. Options:
- Expose the tool with the same two `DateOnly` parameters (simplest, but ask: can Claude reliably produce two well-formed dates from a guest's natural-language request?)
- Take a single string range and parse it inside the tool
- Something else — whatever you choose, the `[Description]` needs to tell Claude exactly what format you expect

Also worth deciding: should the tool surface `HasStopSellActive` explicitly in its description, so Claude knows a "0 rooms" night might be a stop-sell rather than a sellout?

## Run it

```bash
dotnet restore
dotnet build
dotnet run
```

Once your tool compiles and the server starts, use the MCP inspector to call it across a 5–7 night range so you hit both a weekday and a weekend, including the stop-sell night.

```bash
npx @modelcontextprotocol/inspector dotnet run
```

## Stretch goal (workflow pattern)

Sketch a `BackgroundService` that consumes a RabbitMQ message and calls your tool — you don't need a live RabbitMQ connection, a stub consumer that logs the call is enough to demonstrate the pattern.
