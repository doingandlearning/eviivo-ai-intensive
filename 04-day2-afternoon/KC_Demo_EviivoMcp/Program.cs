using EviivoMcp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IEviivoApiClient, EviivoApiClient>();
builder.Services.AddSingleton<INlpService, NlpService>();
builder.Services.AddSingleton<ISupportTicketClient, SupportTicketClient>();

builder.Services
	.AddMcpServer()
	.WithStdioServerTransport()
	.WithToolsFromAssembly()
	.WithPromptsFromAssembly();

var app = builder.Build();
await app.RunAsync();