# 0012. User accounts created on first request; tokens checked for their origin

- Status: Accepted
- Date: 2026-11-16

## Context
Clerk owns identity: sign-up, passwords, Google sign-in and sessions (ADR 0001). BandUp needs its own `User` row to hang profiles, bands and posts off. That row can be created in two ways: through a Clerk webhook when someone signs up, or on the user's first authenticated API request.

Clerk's session tokens include an `azp` (authorised party) claim naming the origin that requested them. Without checking it, a token from another site using the same Clerk instance could be replayed against the API.

## Decision
- **Create the BandUp account on first request.** `GET /api/v1/me` finds the user by their token's `sub` (stored as `AuthSubject`), or creates them. A unique index on `AuthSubject` makes simultaneous first requests safe: the losing request reuses the winner's row.
- **No webhook in MVP 1.** It would need a public endpoint, signature verification and retries, all for something the first request already does. Revisit if we need to react to Clerk-side events, such as deleting a user in the Clerk dashboard (BAND-40 covers account deletion from BandUp).
- **Check `azp`** against `Auth:AuthorizedParties` (`http://localhost:3000` in development, the real web origins in production). Tokens without `azp` (machine tokens) aren't affected.
- `LastSeenAt` is updated at most every 15 minutes, to avoid a database write on every request.
- **Web:** `clerkMiddleware` protects every page except `/`, `/sign-in`, `/sign-up` and `/styleguide`, and the server calls the API with the user's session token. The token never reaches client bundles (`server-only`).

## Consequences
- No webhook infrastructure; the account exists by the time any feature needs it.
- Users who sign up but never open the app have no BandUp row, which is fine.
- `Auth:AuthorizedParties` must be set in every deployed environment (M11 checklist).
- CI needs Clerk test keys and a test user to run signed-in browser tests (repository secrets).
