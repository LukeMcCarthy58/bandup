# 0006. Monorepo with pnpm workspaces

- Status: Accepted
- Date: 2026-11-15

## Context
Solo developer; web, API and later mobile share an API contract.

## Decision
One GitHub repo (`bandup`) with `apps/`, `packages/`, `infra/`, `docs/`. pnpm workspaces for the JS side. CI is path-filtered per app. No Turborepo for now.

## Consequences
- One PR can change the API and the frontend together; a shared generated API client.
- CI must use path filters to stay fast. Add Turborepo only if builds get slow.
