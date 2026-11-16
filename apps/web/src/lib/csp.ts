/**
 * Builds the Content-Security-Policy header. Kept pure so it can be unit tested.
 *
 * - Scripts: only those carrying this request's nonce ('strict-dynamic' lets them
 *   load their own chunks). This is what stops injected <script> tags (XSS).
 * - Styles: 'unsafe-inline' is allowed because React style props, Radix UI and
 *   (later) Clerk set inline style attributes. Style injection is far lower risk
 *   than script injection. See ADR 0009.
 * - Embeds: YouTube and SoundCloud only (posts can embed them).
 * - Clerk: its Frontend API, bot protection and fraud protection hosts (sign-in).
 */
export type CspOptions = {
  nonce: string;
  isDev: boolean;
  /** Extra origins the API lives on, e.g. http://localhost:5080 */
  connectSrc?: string[];
  /** Extra origins images load from, e.g. the CloudFront CDN */
  imgSrc?: string[];
  /** Clerk Frontend API host (from the publishable key), e.g. happy-owl-12.clerk.accounts.dev */
  clerkHost?: string;
};

/** Hosts Clerk needs besides its Frontend API: bot protection and fraud protection. */
export const CLERK_BOT_PROTECTION = "https://challenges.cloudflare.com";
export const CLERK_FRAUD_PROTECTION = "https://*.protect.clerk.com";
export const CLERK_IMAGES = "https://img.clerk.com";

export const EMBED_ORIGINS = [
  "https://www.youtube-nocookie.com",
  "https://www.youtube.com",
  "https://w.soundcloud.com",
];

export function buildCsp({
  nonce,
  isDev,
  connectSrc = [],
  imgSrc = [],
  clerkHost,
}: CspOptions): string {
  const clerk = clerkHost ? `https://${clerkHost}` : undefined;

  const directives: Record<string, string[]> = {
    "default-src": ["'self'"],
    "script-src": [
      "'self'",
      `'nonce-${nonce}'`,
      "'strict-dynamic'",
      // React uses eval for better error stacks in development only.
      ...(isDev ? ["'unsafe-eval'"] : []),
      // Ignored by browsers that support 'strict-dynamic'; a fallback for older ones.
      ...(clerk ? [clerk, CLERK_BOT_PROTECTION, CLERK_FRAUD_PROTECTION] : []),
    ],
    "style-src": ["'self'", "'unsafe-inline'"],
    "img-src": ["'self'", "blob:", "data:", ...imgSrc, ...(clerk ? [CLERK_IMAGES] : [])],
    "font-src": ["'self'"],
    "connect-src": [
      "'self'",
      ...connectSrc,
      ...(clerk ? [clerk, `${CLERK_FRAUD_PROTECTION}:*`] : []),
      ...(isDev ? ["ws:"] : []),
    ],
    "worker-src": ["'self'", "blob:"],
    "frame-src": [
      ...EMBED_ORIGINS,
      ...(clerk ? [CLERK_BOT_PROTECTION, CLERK_FRAUD_PROTECTION] : []),
    ],
    "object-src": ["'none'"],
    "base-uri": ["'self'"],
    "form-action": ["'self'"],
    "frame-ancestors": ["'none'"],
  };

  const policy = Object.entries(directives)
    .map(([name, values]) => `${name} ${values.join(" ")}`)
    .join("; ");

  return isDev ? policy : `${policy}; upgrade-insecure-requests`;
}

/** Origins from a comma-separated env var, ignoring blanks. */
export function originsFromEnv(value: string | undefined): string[] {
  return (value ?? "")
    .split(",")
    .map((origin) => origin.trim())
    .filter(Boolean);
}
