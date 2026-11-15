# BandUp

**LinkedIn for bands.** Musicians find bands, and bands find members. The home page is a social feed that only allows music-related content.

- **MVP 1:** mobile-first web app (current focus)
- **MVP 2:** iOS/Android app (Expo) reusing the same API, plus push notifications

## Features (MVP 1)

- Musician profiles: instruments, genres, town/area, level/commitment, bio, photo
- Bands with members (admin/member roles, current and ex-members)
- Adverts ("looking for a drummer", "guitarist looking for a band"), searchable by distance, instrument and genre
- Join requests, which band admins accept or decline
- Simple message threads (refresh-based)
- Feed of posts (text, images, YouTube/SoundCloud links) as yourself or as a band: Local, Following, Recommended
- Likes, comments, follows
- LLM moderation: every post and comment is checked for music relevance before it goes live, plus reports and an admin queue

Privacy: only town/area is stored and shown, never exact addresses.

## Stack

| Area | Choice |
| --- | --- |
| Web | Next.js (App Router), TypeScript, Tailwind CSS, shadcn/ui |
| API | .NET (LTS) Web API, EF Core, OpenAPI |
| Database | PostgreSQL |
| Auth | Clerk |
| Images | S3 + CloudFront |
| Moderation | Anthropic API (Claude Haiku) |
| Web hosting | Vercel |
| API hosting | Decided at launch (ADR 0002) |
| IaC | Terraform |
| CI/CD | GitHub Actions |
| Mobile (MVP 2) | React Native + Expo |

## Repository layout

```
apps/
  web/              Next.js frontend
  api/              .NET Web API + tests
  mobile/           Expo app (MVP 2)
packages/
  api-client/       TypeScript client generated from OpenAPI
infra/              Terraform (later)
docs/
  adr/              Architecture Decision Records
  spec/             Requirements and API docs
.github/workflows/  CI, path-filtered per app
```

## Getting started

### Prerequisites

- Node.js 24+ and pnpm 11 (`npm install -g pnpm`)
- .NET 10 SDK
- PostgreSQL 18, running locally (no Docker needed; see ADR 0008). If another Postgres already uses port 5432, the installer picks 5433: use whichever port `SHOW port;` reports in your connection string.

### One-time database setup

In pgAdmin (Query Tool on the `postgres` database), run each statement separately:

```sql
CREATE ROLE bandup WITH LOGIN PASSWORD 'your-dev-password';
CREATE DATABASE bandup_dev OWNER bandup;
```

### Web (http://localhost:3000)

```powershell
pnpm install
pnpm dev:web
```

### API (http://localhost:5080/swagger)

```powershell
cd apps/api
dotnet tool restore
dotnet user-secrets set "ConnectionStrings:BandUp" "Host=localhost;Port=5432;Database=bandup_dev;Username=bandup;Password=your-dev-password" --project src/BandUp.Api
dotnet ef database update --project src/BandUp.Infrastructure --startup-project src/BandUp.Api
dotnet run --project src/BandUp.Api
```

More detail: [`apps/web/README.md`](apps/web/README.md) and [`apps/api/README.md`](apps/api/README.md). Design rules: [`docs/design/style-guide.md`](docs/design/style-guide.md).

## Working agreements

- Every change has a Jira ticket (`BAND-n`) with acceptance criteria.
- Branches: `BAND-12-create-band`. Small PRs into `main`, which is protected.
- Tests as we go. ADRs for significant decisions (`docs/adr`).
