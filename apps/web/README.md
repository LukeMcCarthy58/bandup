# web

BandUp's Next.js frontend (App Router, TypeScript, Tailwind CSS v4, shadcn/ui, Clerk).

## Setup

Copy `.env.example` to `.env.local` and fill in the Clerk keys (Clerk dashboard → Configure → API keys). `.env.local` is git-ignored.

```bash
pnpm dev          # http://localhost:3000
pnpm lint
pnpm typecheck
pnpm test         # Vitest + Testing Library
pnpm build
pnpm test:e2e     # Playwright; first run: pnpm exec playwright install chromium
```

## Auth

- Every page except `/`, `/sign-in`, `/sign-up` and `/styleguide` requires sign-in (`src/proxy.ts`).
- Server code calls the API with `apiFetch` (`src/lib/api.ts`), which adds the user's Clerk session token.
- Signed-in e2e tests use a dedicated Clerk test user, set as `E2E_CLERK_USER_EMAIL`.

## Notes

- Design tokens live in `src/app/globals.css` and follow `docs/design/style-guide.md`. The theme follows the OS light/dark setting, and Clerk's UI uses the same tokens.
- Add shadcn components with `pnpm dlx shadcn@latest add <component>`, then restyle them to the tokens.
- Build mobile-first: start at 360px wide and add `sm:`/`md:` styles upward.
- The Content-Security-Policy is built in `src/lib/csp.ts` (ADR 0009). New third-party origins must be added there.
