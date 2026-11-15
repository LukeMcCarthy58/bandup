# 0001. Clerk for authentication

- Status: Accepted
- Date: 2026-11-15

## Context
BandUp needs managed auth for a Next.js web app now and an Expo mobile app later. The .NET API must validate users on every request. Options: AWS Cognito, Clerk.

## Decision
Use Clerk. The web and mobile apps sign in with Clerk SDKs; the API validates Clerk-issued JWTs (JWKS) and maps the Clerk user ID to a local `User` row.

## Consequences
- Fast to ship: polished prebuilt UI and first-class Next.js and Expo SDKs.
- API stays provider-agnostic (standard JWT bearer validation), so switching later is contained.
- Third-party dependency and per-user cost at scale; user data lives partly outside AWS.
