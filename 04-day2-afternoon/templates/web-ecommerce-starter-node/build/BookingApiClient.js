/**
 * Fake-data stub so the lab doesn't depend on real eviivo API access or credentials.
 * Two booking references return hand-written, realistic-looking threads (one a simple
 * late-checkout request, one with an unresolved complaint — useful if your tool needs
 * to demonstrate handling an "escalation-worthy" conversation). Any other booking
 * reference still returns a coherent fallback thread instead of an error, so you can
 * test with arbitrary input.
 */
export class BookingApiClient {
    async getGuestConversation(bookingRef) {
        const now = Date.now();
        const hoursAgo = (h) => new Date(now - h * 3_600_000).toISOString();
        const hoursMinutesAgo = (h, m) => new Date(now - (h * 3_600_000 + m * 60_000)).toISOString();
        if (bookingRef.trim().toUpperCase() === "EVV-2026-00123") {
            return {
                bookingRef: "EVV-2026-00123",
                guestName: "Sarah Whitfield",
                channel: "Airbnb",
                messages: [
                    {
                        timestampUtc: hoursAgo(5),
                        sender: "Guest",
                        text: "Hi! We're arriving on the 2pm train — any chance of a late checkout on our last day instead, rather than early arrival?",
                    },
                    {
                        timestampUtc: hoursMinutesAgo(4, 40),
                        sender: "Host",
                        text: "Hi Sarah, thanks for the heads up. Late checkout until 1pm on departure day works — I'll note it on the booking.",
                    },
                    {
                        timestampUtc: hoursMinutesAgo(4, 35),
                        sender: "Guest",
                        text: "That's perfect, thank you so much!",
                    },
                ],
            };
        }
        if (bookingRef.trim().toUpperCase() === "EVV-2026-00456") {
            return {
                bookingRef: "EVV-2026-00456",
                guestName: "Marco Bellandi",
                channel: "Booking.com",
                messages: [
                    {
                        timestampUtc: hoursAgo(26),
                        sender: "Guest",
                        text: "The room was not as described — there was no sea view as shown in the photos, we are very disappointed.",
                    },
                    {
                        timestampUtc: hoursMinutesAgo(25, 10),
                        sender: "Host",
                        text: "I'm sorry to hear that, Marco. Could you let me know your room number so I can look into this straight away?",
                    },
                    {
                        timestampUtc: hoursAgo(25),
                        sender: "Guest",
                        text: "Room 214. We'd like a partial refund or to be moved if possible.",
                    },
                    {
                        timestampUtc: hoursAgo(3),
                        sender: "Guest",
                        text: "Following up — we haven't heard back and we check out tomorrow.",
                    },
                ],
            };
        }
        // Fallback: still realistic, just generic, so arbitrary booking refs don't error out.
        return {
            bookingRef,
            guestName: "Guest",
            channel: "Email",
            messages: [
                {
                    timestampUtc: hoursAgo(2),
                    sender: "Guest",
                    text: "Hi, just confirming our check-in time for this booking — is 3pm still fine?",
                },
                {
                    timestampUtc: hoursMinutesAgo(1, 45),
                    sender: "Host",
                    text: "Yes, 3pm works well. Looking forward to having you!",
                },
            ],
        };
    }
}
