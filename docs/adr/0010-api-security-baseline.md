# 0010. API security baseline: secure by default

- Status: Accepted
- Date: 2026-11-16

## Context
Every feature adds endpoints. If each endpoint has to remember to require sign-in, validate input, limit abuse and hide errors, one will eventually forget. The rules should be on by default, with deliberate opt-outs.

## Decision
In `BandUp.Api/Security`, wired into every request by `AddApiSecurity`, `UseApiErrorHandling` and `UseApiAccessControl`:

- **Authenticated by default.** A fallback authorisation policy requires a signed-in user. Public endpoints (health, reference data) opt out with `.AllowAnonymous()`. Unknown routes return 401 rather than 404, so the API's shape isn't revealed to anonymous callers.
- **Bearer tokens (JWT)** are validated against the issuer's published keys: signature, issuer, expiry, and audience if set. The issuer is configuration (`Auth:Authority`), set to Clerk in M2.
- **Resource rules as policies.** Checks like "only this band's admins" become named authorisation policies when each feature arrives (e.g. `BandAdmin` in M3). They're never just hidden buttons in the UI.
- **Validation:** .NET 10's built-in minimal API validation (`AddValidation`, with DataAnnotations on request types). It returns 400 with field errors. No extra library is needed.
- **Errors:** RFC 7807 problem details with a `traceId`. No exception messages or stack traces outside Development.
- **Rate limiting:** a global limit per user (or per IP when anonymous), plus named policies (`posting`, `messaging`, `sensitive`) that endpoints opt into. Rejections return 429 with `Retry-After`. All limits are configurable.
- **Headers:** `nosniff`, `X-Frame-Options: DENY`, `Referrer-Policy: no-referrer`, a deny-all CSP, and `Cache-Control: no-store` on `/api`. There's no `Server` header, and HSTS and HTTPS redirection apply outside Development.
- **CORS:** an explicit allow-list of web origins from configuration.
- **Logging:** structured JSON outside Development. Tokens and request bodies are never logged.

## Consequences
- New endpoints are protected without extra code. Forgetting `AllowAnonymous` fails safe (401), not open.
- The baseline is covered by integration tests that use throwaway endpoints, including valid, expired and forged tokens, error leakage, headers and rate limits.
- In-memory rate limits are per server instance. That's fine for one instance; revisit with a shared store if the API scales out (ADR 0002 hosting decision).
