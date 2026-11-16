/**
 * Builds the Content-Security-Policy header. Kept pure so it can be unit tested.
 *
 * - Scripts: only those carrying this request's nonce ('strict-dynamic' lets them
 *   load their own chunks). This is what stops injected <script> tags (XSS).
 * - Styles: 'unsafe-inline' is allowed because React style props, Radix UI and
 *   (later) Clerk set inline style attributes. Style injection is far lower risk
 *   than script injection. See ADR 0009.
 * - Embeds: YouTube and SoundCloud only (posts can embed them).
 */
export type CspOptions = {
  nonce: string;
  isDev: boolean;
  /** Extra origins the API lives on, e.g. http://localhost:5080 */
  connectSrc?: string[];
  /** Extra origins images load from, e.g. the CloudFront CDN */
  imgSrc?: string[];
};

export const EMBED_ORIGINS = [
  "https://www.youtube-nocookie.com",
  "https://www.youtube.com",
  "https://w.soundcloud.com",
];

export function buildCsp({ nonce, isDev, connectSrc = [], imgSrc = [] }: CspOptions): string {
  const directives: Record<string, string[]> = {
    "default-src": ["'self'"],
    "script-src": [
      "'self'",
      `'nonce-${nonce}'`,
      "'strict-dynamic'",
      // React uses eval for better error stacks in development only.
      ...(isDev ? ["'unsafe-eval'"] : []),
    ],
    "style-src": ["'self'", "'unsafe-inline'"],
    "img-src": ["'self'", "blob:", "data:", ...imgSrc],
    "font-src": ["'self'"],
    "connect-src": ["'self'", ...connectSrc, ...(isDev ? ["ws:"] : [])],
    "frame-src": EMBED_ORIGINS,
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
