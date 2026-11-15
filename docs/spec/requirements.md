# BandUp MVP 1 requirements

BandUp is like LinkedIn for bands. Musicians find bands, and bands find members. The home page is a social feed that only allows music-related content.

- **MVP 1:** mobile-first web app (this document)
- **MVP 2:** iOS/Android app (Expo) reusing the same API, plus push notifications

## Users

Musicians, signed in with Clerk (ADR 0001). Some users are also **site admins**, who moderate content. This role is separate from band admin.

## Functional requirements

| # | Requirement | Epic |
| --- | --- | --- |
| F1 | Create a profile: name, instruments, genres, town/area, level/commitment, bio, photo | Auth & Profiles |
| F2 | Create bands. A user can be in many bands; a band has many members | Bands & Membership |
| F3 | Memberships have a role (admin/member) and status (current/ex-member, with joined/left dates); bands show ex-members | Bands & Membership |
| F4 | Bands post "looking for a member" adverts; musicians post "looking for a band" adverts | Adverts & Location Search |
| F5 | Browse adverts by distance first (radius in miles from a town), then filter by instrument and genre | Adverts & Location Search |
| F6 | Request to join a band from an advert; band admins accept or decline | Join Requests |
| F7 | Simple message threads, one per join request or advert reply. Refresh-based, not real-time | Messaging |
| F8 | Post text, images and YouTube/SoundCloud links, as yourself or as a band you're an admin of. Each post stores the real author and who it's posted as | Posts & Feed |
| F9 | Like and comment on posts | Likes, Comments & Follows |
| F10 | Follow users and bands | Likes, Comments & Follows |
| F11 | Switch the feed between Local, Following and Recommended (simple rules: nearby, matching genres/instruments, popular) | Posts & Feed |
| F12 | Delete your account and data | Auth & Profiles |
| F13 | Block a user from messaging you | Messaging |

## Non-functional requirements

| Area | Requirement |
| --- | --- |
| Mobile-first | Designed from 360px wide upwards; 44px touch targets; bottom tab bar on mobile (ADR 0007) |
| Accessibility | WCAG 2.2 AA: contrast, keyboard access, focus styles, alt text, reduced motion |
| Moderation | Every post, comment and advert is checked by an LLM for music relevance before it goes live. If the provider fails, content goes to review; it is never auto-approved. User reports and an admin review queue act as a backup |
| Privacy | Store and show town/area only, never exact addresses. Strip image location data (EXIF/GPS). Right to erasure (UK GDPR) |
| Performance | Feed and search API responses under ~300ms (p95); images served via CDN |
| Security | Managed auth; server-side permission checks on every action (only band admins can manage or post as a band); pre-signed S3 uploads; input validation; rate limiting on posting, messaging and reports; security headers and CSP; dependency and code scanning |
| Portability | The API is client-agnostic, so the MVP 2 mobile app reuses it |
| Quality | Tests as we go (unit, integration, component, e2e); CI on every PR; ADRs for significant decisions |

## Out of scope for MVP 1

AI-powered band search, audio/video uploads, real-time chat, AI-ranked feed, mobile app, push notifications.
