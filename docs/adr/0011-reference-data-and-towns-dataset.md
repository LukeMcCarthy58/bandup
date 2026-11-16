# 0011. Reference data and the towns dataset

- Status: Accepted
- Date: 2026-11-16

## Context
Profiles, bands and adverts pick from fixed lists of instruments, genres and towns. Towns also drive distance search, and must never reveal more than "roughly where" someone is. We need a UK towns dataset with an open licence.

## Decision
- **Instruments and genres** are curated lists seeded by EF Core migrations (`ReferenceSeed`), with fixed integer Ids that are never renumbered. Instruments keep a curated picker order; genres are alphabetical.
- **Towns** store name, region, country and a centroid (latitude/longitude) plus a normalised `SearchName`, which has a `text_pattern_ops` index for fast prefix search (`LIKE 'man%'`).
- A **starter set of 78 major UK towns** is seeded now, so local development and tests work offline.
- The **full dataset comes from GeoNames** (UK populated places, CC BY 4.0, attribution required), loaded by an import command in M4 (BAND-77). Seeded Ids stay stable.
- The API **never returns coordinates**: town results contain only id, name, region and country (`TownSummary`).
- Reference endpoints (`/api/v1/instruments`, `/genres`, `/towns?query=`) are public and cacheable (instruments and genres for an hour).

## Consequences
- Pickers and autocomplete work from day one; smaller towns are missing until BAND-77.
- Changing reference data means a new migration, which keeps every environment consistent.
- GeoNames attribution must appear in the app once the full dataset is in.
- Alternatives considered: the ONS Index of Place Names (OGL, England and Wales focused, with more cleaning needed) and a geocoding API (cost, a network dependency on the write path, and privacy).
