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

Tests tagged `Category=Database` use the `ConnectionStrings__BandUp` environment variable when it's set. CI provides a Postgres service container. Without it, those tests check that the API reports the database as unavailable rather than crashing.
