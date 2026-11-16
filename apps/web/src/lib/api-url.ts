const DEFAULT_API_URL = "http://localhost:5080";

/**
 * BandUp's own API origin (e.g. http://localhost:5080), checked so a wrong setting fails
 * loudly with a clear message instead of every page saying "couldn't load".
 *
 * A common mix-up is pasting Clerk's "Frontend API URL" here (BAND-84): that's Clerk's
 * server, not ours, and every call to it returns 404.
 */
export function checkApiUrl(value: string | undefined, name = "NEXT_PUBLIC_API_URL"): string {
  const raw = value?.trim() || DEFAULT_API_URL;

  let url: URL;
  try {
    url = new URL(raw);
  } catch {
    throw new Error(`${name} must be a full URL such as ${DEFAULT_API_URL}, but it's "${raw}".`);
  }

  if (url.protocol !== "http:" && url.protocol !== "https:") {
    throw new Error(`${name} must start with http:// or https://, but it's "${raw}".`);
  }
  if (url.hostname.endsWith(".clerk.accounts.dev") || url.hostname.includes("clerk.")) {
    throw new Error(
      `${name} points at Clerk (${url.hostname}). It should be BandUp's API, e.g. ${DEFAULT_API_URL}.`,
    );
  }

  // Paths are added by the client (/api/v1/...), so keep the origin only.
  return url.origin;
}

/** The API origin for server code. A private URL (API_URL) wins if one is set. */
export const serverApiUrl = () =>
  process.env.API_URL
    ? checkApiUrl(process.env.API_URL, "API_URL")
    : checkApiUrl(process.env.NEXT_PUBLIC_API_URL);

/** The API origin the browser calls (public endpoints only, such as town search). */
export const publicApiUrl = () => checkApiUrl(process.env.NEXT_PUBLIC_API_URL);
