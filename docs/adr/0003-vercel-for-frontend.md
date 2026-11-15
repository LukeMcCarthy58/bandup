# 0003. Vercel for the Next.js frontend

- Status: Accepted
- Date: 2026-11-15

## Context
Options: Vercel, AWS Amplify Hosting, AWS via OpenNext/SST.

## Decision
Host the web app on Vercel.

## Consequences
- Zero-config Next.js support and a preview deploy per PR.
- Vercel's Hobby plan is for non-commercial use; move to Pro if BandUp becomes commercial.
- Frontend sits outside AWS; the API's CORS and auth must allow the Vercel domains.
