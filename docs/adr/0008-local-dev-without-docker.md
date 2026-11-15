# 0008. Local development without Docker; distance search without PostGIS

- Status: Accepted
- Date: 2026-11-15

## Context
Luke doesn't use Docker on his laptop. The original plan used docker-compose for Postgres and LocalStack for S3. Radius search ("adverts within N miles") is often built on PostGIS, which is awkward to install natively on Windows.

## Decision
- Run PostgreSQL 18 natively on Windows. The API connects as a least-privilege `bandup` role that owns `bandup_dev`.
- Keep the connection string in .NET user-secrets, never in the repo.
- CI uses a Postgres service container on GitHub Actions, so no local Docker is needed.
- Store town/area centroids as indexed latitude/longitude. Radius search filters with a bounding box (index-friendly), then ranks by Haversine distance, with no PostGIS.
- For S3 in local dev, use a small real dev bucket with a least-privilege IAM user when the Images epic starts. Unit tests use an `IImageStorage` fake.

## Consequences
- Simpler laptop setup, and offline development works.
- Local Postgres version must be kept in step with CI and production (pin 18).
- Bounding box + Haversine is accurate enough for town-level distances and fast at our scale. Revisit PostGIS if we need polygons or millions of rows.
- The dev S3 bucket needs an AWS account by the Images epic, which is earlier than other AWS work.
