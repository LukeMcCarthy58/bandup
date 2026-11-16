/**
 * Works out the Clerk Frontend API host from the publishable key, so the CSP can
 * allow exactly that host. A key looks like `pk_test_<base64("host$")>`.
 */
export function clerkFrontendApiHost(publishableKey: string | undefined): string | undefined {
  const encoded = publishableKey?.split("_")[2];
  if (!encoded) return undefined;

  try {
    const decoded = atob(encoded);
    const host = decoded.endsWith("$") ? decoded.slice(0, -1) : decoded;
    return /^[a-z0-9.-]+$/i.test(host) ? host : undefined;
  } catch {
    return undefined;
  }
}
