import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { BookingApiClient } from "./BookingApiClient.js";
import { registerBookingTools } from "./BookingTools.js";

// Equivalent of the DI registration in the .NET starters' Program.cs — construct the
// stub API client once and hand it to the tools module.
const api = new BookingApiClient();

const server = new McpServer({
  name: "web-ecommerce-mcp",
  version: "1.0.0",
});

registerBookingTools(server, api);

const transport = new StdioServerTransport();
await server.connect(transport);

// stdio is reserved for JSON-RPC — never console.log() in a stdio server, it corrupts
// the protocol stream. console.error() writes to stderr and is safe to use.
console.error("Web/eCommerce MCP server running on stdio");
