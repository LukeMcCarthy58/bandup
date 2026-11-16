import createClient, { type Client, type Middleware } from "openapi-fetch";

import type { components, paths } from "./generated/schema";

export type { components, paths };

/** Every request and response shape in the API, e.g. `Schemas["ProfileView"]`. */
export type Schemas = components["schemas"];

export type BandUpClient = Client<paths>;

export type BandUpClientOptions = {
  /** Where the API runs, e.g. http://localhost:5080. */
  baseUrl: string;
  /** Returns the signed-in user's session token, or null when signed out. */
  getToken?: () => Promise<string | null>;
};

/**
 * Creates a typed client: paths, parameters, bodies and responses are all checked
 * against the API's OpenAPI document at compile time.
 */
export function createBandUpClient({ baseUrl, getToken }: BandUpClientOptions): BandUpClient {
  const client = createClient<paths>({ baseUrl });

  if (getToken) {
    const auth: Middleware = {
      async onRequest({ request }) {
        const token = await getToken();
        if (token) request.headers.set("Authorization", `Bearer ${token}`);
        return request;
      },
    };
    client.use(auth);
  }

  return client;
}
