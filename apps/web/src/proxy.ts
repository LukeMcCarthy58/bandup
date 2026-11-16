import { clerkMiddleware, createRouteMatcher } from "@clerk/nextjs/server";
import { NextResponse, type NextRequest } from "next/server";

import { clerkFrontendApiHost } from "@/lib/clerk";
import { buildCsp, originsFromEnv } from "@/lib/csp";

/** Pages anyone can see. Everything else needs a signed-in user. */
const isPublicRoute = createRouteMatcher(["/", "/sign-in(.*)", "/sign-up(.*)", "/styleguide"]);

/**
 * Runs before every page request:
 * 1. Clerk works out who the user is, and sends signed-out users to /sign-in.
 * 2. A fresh nonce and a strict Content-Security-Policy are set (ADR 0009).
 *    Next.js and Clerk both read the nonce from the request header.
 */
export default clerkMiddleware(
  async (auth, request) => {
    if (!isPublicRoute(request)) {
      await auth.protect();
    }

    return withContentSecurityPolicy(request);
  },
  { signInUrl: "/sign-in", signUpUrl: "/sign-up" },
);

function withContentSecurityPolicy(request: NextRequest) {
  const nonce = Buffer.from(crypto.randomUUID()).toString("base64");
  const csp = buildCsp({
    nonce,
    isDev: process.env.NODE_ENV === "development",
    connectSrc: originsFromEnv(process.env.NEXT_PUBLIC_API_URL),
    imgSrc: originsFromEnv(process.env.NEXT_PUBLIC_IMAGE_CDN_URL),
    clerkHost: clerkFrontendApiHost(process.env.NEXT_PUBLIC_CLERK_PUBLISHABLE_KEY),
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
    // Skip Next.js internals and static files.
    "/((?!_next|[^?]*\\.(?:html?|css|js(?!on)|jpe?g|webp|png|gif|svg|ttf|woff2?|ico|csv|docx?|xlsx?|zip|webmanifest)).*)",
    "/(api|trpc)(.*)",
    // Clerk's own routes.
    "/__clerk/(.*)",
  ],
};
