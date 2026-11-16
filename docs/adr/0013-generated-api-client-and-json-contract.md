# 0013. A generated, typed API client, and the JSON contract it relies on

- Status: Accepted
- Date: 2026-11-16

## Context
The web app (and later the Expo app) calls the .NET API. Hand-written `fetch` calls and hand-copied types drift from the API without anyone noticing until something breaks at runtime. The API already describes itself with an OpenAPI document (`Microsoft.AspNetCore.OpenApi`).

## Decision
- **The API's build writes `packages/api-client/openapi.json`** (`Microsoft.Extensions.ApiDescription.Server`). Generation starts the app without a database, so `AddInfrastructure` uses a placeholder connection string only when the entry assembly is `GetDocument.Insider`.
- **`@bandup/api-client` generates TypeScript types from it** with `openapi-typescript`, and wraps `openapi-fetch` in `createBandUpClient({ baseUrl, getToken })`, which adds the Clerk bearer token. Both generated files are committed, so the web build needs no .NET.
- **CI fails if either file is stale:** the API job checks `openapi.json` after building, and the web job regenerates the types and checks for a diff.
- **JSON contract:** enums are strings (`"Intermediate"`, not `2`), and numbers are strict JSON numbers, so the document says `integer`, not `integer | string`.
- The web app only calls the API from server code (`serverApi()` is `server-only`), so tokens never reach the browser. Public endpoints such as town search can be called from the browser directly.

## Consequences
- A change to an endpoint shows up as a TypeScript error in the web app, not a runtime bug.
- After changing the API: `dotnet build`, then `pnpm --filter @bandup/api-client generate`, and commit both files.
- Validation messages are written for users in the API (`[Required(ErrorMessage = "Enter your name.")]`) and shown as-is next to the right field.
- Renaming an enum value is a breaking API change, and so needs a migration of stored values too (they're stored as strings).
