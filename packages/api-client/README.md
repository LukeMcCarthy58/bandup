# @bandup/api-client

A typed client for the BandUp API, generated from its OpenAPI document. Shared by `apps/web` and, later, `apps/mobile`.

- `openapi.json` is written by the API's build (`dotnet build` in `apps/api`). **Don't edit it by hand.**
- `src/generated/schema.d.ts` is generated from it. **Don't edit it by hand either.**

After changing an API endpoint:

```powershell
cd apps/api; dotnet build             # refreshes openapi.json
pnpm --filter @bandup/api-client generate
```

Commit both files. CI fails if either is out of date with the API.

```ts
import { createBandUpClient } from "@bandup/api-client";

const api = createBandUpClient({ baseUrl, getToken });
const { data, error } = await api.GET("/api/v1/users/{id}", { params: { path: { id } } });
```
