import { z } from "zod";
import type { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import type { IBookingApiClient } from "./IBookingApiClient.js";

// TODO (squad): this is where your tool lives. `api` is ready to use —
// your job is the server.tool(...) call below.
//
// Reminder from the lab: write the description before you write the implementation.
// If you can't say in one sentence what this tool does and when Claude should call it,
// stop and talk it through with your squad first.
export function registerBookingTools(server: McpServer, api: IBookingApiClient): void {
  // Delete this comment block and write your tool here. Your starting signature
  // (from the lab README) is:
  //
  //     getGuestConversation(bookingRef)
  //
  // Try it against "EVV-2026-00123" (a simple late-checkout request) and
  // "EVV-2026-00456" (an unresolved complaint) — they exercise different scenarios.
  // Any other booking ref currently returns null; your tool should handle that case
  // explicitly (e.g., return a "not found" payload instead of serializing null blindly).
  //
  // Example shape to follow — uncomment and adapt, don't just fill in blindly:
  //
  // server.tool(
  //   "get_guest_conversation",
  //   "One sentence: what this tool does. One sentence: when Claude should call it. " +
  //     "One sentence: any important constraints or caveats.",
  //   {
  //     bookingRef: z
  //       .string()
  //       .describe("The eviivo booking reference, e.g. EVV-2026-00123"),
  //   },
  //   async ({ bookingRef }) => {
  //     const conversation = await api.getGuestConversation(bookingRef);
  //     if (conversation === null) {
  //       return {
  //         content: [{ type: "text", text: JSON.stringify({ bookingRef, found: false }, null, 2) }],
  //       };
  //     }
  //     return {
  //       content: [{ type: "text", text: JSON.stringify({ found: true, conversation }, null, 2) }],
  //     };
  //   },
  // );
}
