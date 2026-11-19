# api

BandUp's .NET 10 Web API: minimal APIs, EF Core and PostgreSQL, with an OpenAPI document and Swagger UI in Development.

## Layout (clean architecture)

```
src/
  BandUp.Domain/          Entities and rules. No dependencies.
  BandUp.Application/     Use cases and ports (e.g. IModerationService). Depends on Domain.
  BandUp.Infrastructure/  Adapters: EF Core/Postgres, later S3, Anthropic, Clerk.
  BandUp.Api/             HTTP endpoints, auth and composition root.
tests/
  BandUp.UnitTests/         Fast tests for Domain and Application.
  BandUp.IntegrationTests/  Boots the API in memory (WebApplicationFactory).
```

Dependencies point inwards: Api → Application → Domain, and Infrastructure → Application. Domain never references EF Core or ASP.NET.

## First-time setup

You need the .NET 10 SDK and PostgreSQL 18, with a `bandup_dev` database owned by the `bandup` role (see the root README).

```powershell
cd apps/api
dotnet tool restore
dotnet user-secrets set "ConnectionStrings:BandUp" "Host=localhost;Port=5432;Database=bandup_dev;Username=bandup;Password=YOUR_DEV_PASSWORD" --project src/BandUp.Api
dotnet ef database update --project src/BandUp.Infrastructure --startup-project src/BandUp.Api
```

The connection string lives in .NET user-secrets, outside the repo, so it's never committed.

## Everyday commands

```powershell
dotnet build
dotnet test
dotnet run --project src/BandUp.Api   # http://localhost:5080/swagger
```

| Endpoint | Purpose |
| --- | --- |
| `GET /health/live` | Process is up (no dependencies checked) |
| `GET /health` | Ready: includes a database check (503 if unreachable) |
| `GET /openapi/v1.json` | OpenAPI document (Development only) |
| `/swagger` | Swagger UI (Development only) |

## Migrations

```powershell
dotnet ef migrations add <Name> --project src/BandUp.Infrastructure --startup-project src/BandUp.Api --output-dir Persistence/Migrations
dotnet ef database update --project src/BandUp.Infrastructure --startup-project src/BandUp.Api
```

## Tests and the database

Integration tests run against a separate **`bandup_test`** database, so they never touch your dev data. They reuse the server, user and password from your user-secret (or the `ConnectionStrings__BandUp` environment variable in CI), swap the database name, and apply migrations automatically.

One-time setup, in pgAdmin's Query Tool (on its own):

```sql
CREATE DATABASE bandup_test OWNER bandup;
```

## Auth

The API accepts Clerk session tokens (`Authorization: Bearer …`). Set the issuer once:

```powershell
dotnet user-secrets set "Auth:Authority" "https://<your-frontend-api>.clerk.accounts.dev" --project src/BandUp.Api
```

`GET /api/v1/me` returns the signed-in user's BandUp account, creating it on the first call (ADR 0012). Integration tests sign their own tokens (`TestTokens`), so they never call Clerk.

## Reference data endpoints (public)

| Endpoint | Returns |
| --- | --- |
| `GET /api/v1/instruments` | All instruments, in picker order |
| `GET /api/v1/genres` | All genres, alphabetical |
| `GET /api/v1/towns?query=man&limit=10` | Towns starting with the query (2 to 50 characters, max 20 results). Never includes coordinates |

## Profile endpoints (signed in)

| Endpoint | Does |
| --- | --- |
| `GET /api/v1/me/profile` | Your profile. `hasProfile` is false until you've set your name, town and at least one instrument |
| `PUT /api/v1/me/profile` | Creates or replaces your profile. Returns field errors (`400`) written for users. Rate limited (`posting`) |
| `DELETE /api/v1/me` | Permanently deletes your BandUp account and profile. `204`, even if already deleted. Rate limited (`sensitive`). See ADR 0014 |
| `GET /api/v1/users/{id}` | Someone's public profile: town name only, never coordinates. `404` if they haven't set one up |

Enums are sent as strings (`"Intermediate"`). The OpenAPI document is written to `packages/api-client/openapi.json` on every build; see ADR 0013.
