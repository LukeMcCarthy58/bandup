import "server-only";

import { auth } from "@clerk/nextjs/server";
import { createBandUpClient, type BandUpClient } from "@bandup/api-client";

/** Where the API runs. Server code may use a private URL (API_URL) if one is set. */
const baseUrl = process.env.API_URL ?? process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5080";

/**
 * A typed API client for server code, sending the signed-in user's Clerk session token.
 * Server-only, so tokens never reach client bundles.
 */
export async function serverApi(): Promise<BandUpClient> {
  const { getToken } = await auth();
  return createBandUpClient({ baseUrl, getToken: () => getToken() });
}
