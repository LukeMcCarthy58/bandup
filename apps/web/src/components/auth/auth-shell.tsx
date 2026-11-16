import Link from "next/link";
import type { ReactNode } from "react";

/** Centred, mobile-first frame for the sign-in and sign-up pages. */
export function AuthShell({ children }: { children: ReactNode }) {
  return (
    <main
      id="main"
      className="flex flex-1 flex-col items-center px-4 pt-[calc(2rem+env(safe-area-inset-top))] pb-10 sm:justify-center sm:pt-10"
    >
      <Link href="/" className="mb-6 font-heading text-3xl font-bold text-primary">
        BandUp
      </Link>
      {children}
    </main>
  );
}
