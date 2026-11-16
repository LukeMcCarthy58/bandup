import "server-only";

import { auth } from "@clerk/nextjs/server";
import { createBandUpClient, type BandUpClient, type Middleware } from "@bandup/api-client";

import { serverApiUrl } from "@/lib/api-url";

/**
 * Logs failed API calls on the server (method, path, status and the problem's title), so a
 * "couldn't load" page always has a cause in the terminal. Never logs tokens or bodies.
 */
const logFailures: Middleware = {
  async onResponse({ request, response }) {
    if (response.ok) return;
    let title = "";
    try {
      const problem = (await response.clone().json()) as { title?: string; detail?: string };
      title = [problem.title, problem.detail].filter(Boolean).join(": ");
    } catch {
      // Not a problem-details body; the status is enough.
    }
    console.error(
      `BandUp API ${request.method} ${new URL(request.url).pathname} returned ${response.status}${title ? ` (${title})` : ""}`,
    );
  },
  onError({ request, error }) {
    console.error(`BandUp API ${request.method} ${new URL(request.url).pathname} failed`, error);
  },
};

/**
 * A typed API client for server code, sending the signed-in user's Clerk session token.
 * Server-only, so tokens never reach client bundles.
 */
export async function serverApi(): Promise<BandUpClient> {
  const { getToken } = await auth();
  const client = createBandUpClient({ baseUrl: serverApiUrl(), getToken: () => getToken() });
  client.use(logFailures);
  return client;
}
