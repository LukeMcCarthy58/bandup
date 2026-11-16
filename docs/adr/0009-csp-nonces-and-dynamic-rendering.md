# 0009. Nonce-based Content Security Policy, with per-request rendering

- Status: Accepted
- Date: 2026-11-16

## Context
A Content Security Policy (CSP) is the main browser-side defence against cross-site scripting (XSS): it tells the browser which scripts may run. BandUp shows a lot of user-generated content (posts, comments, adverts, messages), so XSS is a real risk.

Next.js offers three approaches:
1. **Nonces:** a random value per request, added to every allowed script. The strongest option, but every page must be rendered per request rather than prebuilt.
2. **Subresource Integrity:** hashes computed at build time, which keeps pages static. Still experimental, and it doesn't cover the inline scripts Next.js generates.
3. **No nonce, with `'unsafe-inline'` scripts:** easy, but it gives little protection against XSS.

## Decision
- Use a **nonce-based CSP**, set in `src/proxy.ts`, with the policy built by a pure, unit-tested function (`src/lib/csp.ts`).
- `script-src 'self' 'nonce-…' 'strict-dynamic'`, with no `unsafe-inline`. `unsafe-eval` is allowed in development only.
- `style-src` allows `'unsafe-inline'`, because React style props, Radix UI and Clerk set inline styles. Injected styles are far less dangerous than injected scripts.
- `frame-src` allows only the YouTube and SoundCloud embed origins. `frame-ancestors 'none'`, `object-src 'none'` and `base-uri 'self'` are set as well.
- The API and image CDN origins come from `NEXT_PUBLIC_API_URL` and `NEXT_PUBLIC_IMAGE_CDN_URL`.
- Every page renders per request: the root layout reads the request headers, and Cache Components is turned off.
- Static headers (HSTS, `X-Content-Type-Options`, `Referrer-Policy`, `X-Frame-Options`, `Permissions-Policy`) stay in `next.config.ts`.

## Consequences
- Strong XSS protection from day one, which matters for a user-generated-content app.
- Pages can't be served from a CDN cache. That's acceptable because nearly every BandUp page is personalised once sign-in exists. Images and static assets are still cached.
- Each new third-party origin (Clerk in BAND-34, the image CDN, analytics) must be added to the CSP deliberately. A blocked resource shows up as a console error, and the unit tests document the allowed origins.
- Revisit Cache Components for public pages (like the landing page) if performance needs it.
