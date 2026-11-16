import { NextResponse, type NextRequest } from "next/server";

import { buildCsp, originsFromEnv } from "@/lib/csp";

/**
 * Runs before every page request: generates a fresh nonce and sets a strict
 * Content-Security-Policy. Next.js reads the nonce from the request header and
 * attaches it to its own scripts automatically.
 */
export function proxy(request: NextRequest) {
  const nonce = Buffer.from(crypto.randomUUID()).toString("base64");
  const csp = buildCsp({
    nonce,
    isDev: process.env.NODE_ENV === "development",
    connectSrc: originsFromEnv(process.env.NEXT_PUBLIC_API_URL),
    imgSrc: originsFromEnv(process.env.NEXT_PUBLIC_IMAGE_CDN_URL),
  });

  const requestHeaders = new Headers(request.headers);
  requestHeaders.set("x-nonce", nonce);
  requestHeaders.set("Content-Security-Policy", csp);

  const response = NextResponse.next({ request: { headers: requestHeaders } });
  response.headers.set("Content-Security-Policy", csp);
  return response;
}

export const config = {
  matcher: [
    {
      // Pages only: skip static files, image optimisation and prefetches.
      source:
        "/((?!api|_next/static|_next/image|favicon.ico|.*\\.(?:svg|png|jpg|jpeg|webp|ico)$).*)",
      missing: [
        { type: "header", key: "next-router-prefetch" },
        { type: "header", key: "purpose", value: "prefetch" },
      ],
    },
  ],
};
