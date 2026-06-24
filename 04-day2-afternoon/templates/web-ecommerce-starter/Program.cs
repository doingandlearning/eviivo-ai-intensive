using WebEcommerceMcp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Register the stub API client BEFORE calling AddMcpServer() — if DI isn't resolving
// your tool's constructor, this ordering is the first thing to check.
builder.Services.AddSingleton<IBookingApiClient, BookingApiClient>();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
