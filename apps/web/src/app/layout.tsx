import type { Metadata, Viewport } from "next";
import { headers } from "next/headers";
import "@fontsource-variable/inter";
import "@fontsource-variable/bricolage-grotesque";
import "./globals.css";

import { SkipLink } from "@/components/app-shell/skip-link";

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
  // Reading the request renders every page per request, so Next.js can attach
  // the CSP nonce set in src/proxy.ts to its scripts (ADR 0009).
  await headers();

  return (
    <html lang="en-GB">
      <body className="flex min-h-dvh flex-col">
        <SkipLink />
        {children}
      </body>
    </html>
  );
}
