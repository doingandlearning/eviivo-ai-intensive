/**
 * Stub of eviivo's unified guest inbox (the API behind the Guest Manager — email,
 * WhatsApp, SMS, and OTA messaging in one thread). Swap this implementation for a real
 * HTTP client once you're pointing at an actual environment — the interface is the
 * contract your MCP tool depends on, so the tool function shouldn't need to change.
 */
export interface IBookingApiClient {
  getGuestConversation(bookingRef: string): Promise<GuestConversation | null>;
}

export interface GuestMessage {
  timestampUtc: string;
  sender: "Guest" | "Host";
  text: string;
}

export interface GuestConversation {
  bookingRef: string;
  guestName: string;
  channel: string;
  messages: GuestMessage[];
}
