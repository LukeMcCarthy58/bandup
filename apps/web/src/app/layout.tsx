import { ClerkProvider } from "@clerk/nextjs";
import type { Metadata, Viewport } from "next";
import { headers } from "next/headers";
import "@fontsource-variable/inter";
import "@fontsource-variable/bricolage-grotesque";
import "./globals.css";

import { SkipLink } from "@/components/app-shell/skip-link";
import { clerkAppearance } from "@/components/auth/clerk-appearance";

export const metadata: Metadata = {
  title: {
    default: "BandUp",
    template: "%s · BandUp",
  },
  description: "Find your band. Musicians find bands, and bands find members.",
};

export const viewport: Viewport = {
  width: "device-width",
  initialScale: 1,
  viewportFit: "cover",
  colorScheme: "light dark",
  themeColor: [
    { media: "(prefers-color-scheme: light)", color: "#ffffff" },
    { media: "(prefers-color-scheme: dark)", color: "#18181d" },
  ],
};

export default async function RootLayout({ children }: LayoutProps<"/">) {
  // Reading the request renders every page per request, so Next.js and Clerk can
  // attach the CSP nonce set in src/proxy.ts to their scripts (ADR 0009).
  await headers();

  return (
    <html lang="en-GB">
      <body className="flex min-h-dvh flex-col">
        <ClerkProvider
          dynamic
          appearance={clerkAppearance}
          signInUrl="/sign-in"
          signUpUrl="/sign-up"
          signInFallbackRedirectUrl="/feed"
          signUpFallbackRedirectUrl="/me"
          afterSignOutUrl="/"
        >
          <SkipLink />
          {children}
        </ClerkProvider>
      </body>
    </html>
  );
}
