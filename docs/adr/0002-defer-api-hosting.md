# 0002. Defer API hosting; local-only until launch

- Status: Accepted
- Date: 2026-11-15

## Context
Candidates are AWS Lambda, ECS Fargate and App Runner. Traffic is zero until launch, and choosing now adds cost and setup with no benefit.

## Decision
Build and run the API locally (Docker Postgres) until MVP 1 is feature-complete. Choose hosting in a later ADR. Keep the API stateless and container-friendly so any option works.

## Consequences
- No cloud cost or infra work during the build.
- Must avoid assumptions tied to one host (e.g. local file storage, in-memory sessions).
- Images still need S3; LocalStack or a dev bucket covers that when we reach the Images epic.
