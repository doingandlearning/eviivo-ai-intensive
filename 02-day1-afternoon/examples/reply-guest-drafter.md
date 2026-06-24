---
name: reply-guest-message
version: 1.2
owner: connectivity-squad
purpose: Generate a first-response to a guest message in the eviivo Concierge inbox
inputs:
  - message: string (the guest's raw message text)
  - property_name: string
  - tone_profile: enum [professional, friendly, formal] 
    — default: friendly
  - booking_state: enum [pre-arrival, in-stay, post-stay, no-booking] — required
outputs:
  - reply_text: string (max 120 words)
  - escalate_flag: boolean
failure_modes:
  - empty message → return null, do not reply
  - unknown booking_state → escalate_flag: true, log warning
tested: true
last_tested: 2026-05-12
---

# reply-guest-message v1.2
Owner: Connectivity squad
Location: skills/reply-guest-message.md (Connectivity squad repo)
Last reviewed: 2026-05-12
Trigger: A new inbound guest message arrives in the eviivo Concierge inbox and no human agent has already claimed the conversation.

## Purpose
Draft a first-response reply to an inbound guest message, and flag the conversation for human escalation when sentiment, booking state, or message content indicate it shouldn't be auto-replied to in full.

## Context (system prompt additions)
- Property name and the property's tone profile (professional / friendly / formal — default: friendly)
- The guest's current booking state (pre-arrival / in-stay / post-stay / no-booking)
- The last 3 messages in this conversation thread, if any (an empty list for a first-time contact is normal, not an error)
- Reply must stay under 120 words
- The model must NOT promise compensation, refunds, or specific maintenance timelines — those require human sign-off
- The model must NOT attempt to resolve the guest's underlying issue — only acknowledge it and set expectations

## Tools available
- `classify_sentiment(message: string)` — returns a sentiment score from -1 (very negative) to 1 (very positive); backed by Haiku 4.5, shared across Skills, not reimplemented here
- `get_booking_state(booking_id: string)` — returns the current `booking_state` enum, or null if not found

## Inputs
- `message`: string — required — the guest's raw inbound message text
- `property_name`: string — optional — falls back to "your property" if missing; logs a warning, does not block the reply
- `tone_profile`: enum [professional, friendly, formal] — optional — defaults to "friendly" if not set at the property level
- `last_3_interactions`: list\<string\> — optional — defaults to an empty list for first-time guests
- `booking_state`: enum [pre-arrival, in-stay, post-stay, no-booking] — required — resolved via `get_booking_state`; see error handling if null

## Expected output
- `reply_text`: string, max 120 words — either a full reply (normal path) or a short holding reply (escalation path: acknowledges the message and states a team member will follow up — does not attempt resolution)
- `escalate_flag`: boolean
- `escalate_reason`: string — populated only when `escalate_flag` is true; one of `negative_sentiment`, `unknown_booking_state`, `distress_keyword_match`

## Error handling
- If `message` is empty or null: return `reply_text: null`, `escalate_flag: false`, log "empty message received" — do not send anything
- If `get_booking_state` returns null or errors: `escalate_flag: true`, `escalate_reason: "unknown_booking_state"`, draft a holding reply only
- If `classify_sentiment` score is below -0.4: `escalate_flag: true`, `escalate_reason: "negative_sentiment"`, draft a holding reply only
- If the message contains a distress keyword ("review", "no response", "complaint", "refund") regardless of sentiment score: `escalate_flag: true`, `escalate_reason: "distress_keyword_match"`, draft a holding reply only
- If `property_name` is missing: fall back to "your property", log a warning, continue normally — this is not an escalation condition

## Test cases
| Input | Expected output |
|---|---|
| message: "What time is checkout tomorrow?" · booking_state: in-stay · sentiment: neutral | Full reply stating checkout time, tone-appropriate sign-off. `escalate_flag: false` |
| message: "Room smells of cigarettes, kids sleeping in it, no one's responded to my last 3 messages, I'm leaving a review." · booking_state: null | `escalate_flag: true` · `escalate_reason: "unknown_booking_state"` (also matches a distress keyword) · `reply_text` is a holding reply only |
| message: "" (empty) | `reply_text: null` · `escalate_flag: false` · logged as empty message, nothing sent |

## Change log
- v1.0 — 2026-03-02 — initial version (Connectivity squad) — sentiment score only, no keyword check
- v1.1 — 2026-04-10 — added the unknown-`booking_state` escalation path
- v1.2 — 2026-05-12 — added the distress-keyword check after a borderline-sentiment complaint slipped through in v1.1; added `escalate_reason` for testability
