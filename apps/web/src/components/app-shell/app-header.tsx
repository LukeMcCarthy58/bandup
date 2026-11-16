"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { Search } from "lucide-react";

import { cn } from "@/lib/utils";

import { isActive, navItems } from "./nav-items";

/**
 * Top bar. On mobile: logo and search (the tabs live in the bottom bar).
 * From md up: logo, the full navigation, and a Create button.
 */
export function AppHeader() {
  const pathname = usePathname();

  return (
    <header className="sticky top-0 z-40 border-b bg-card pt-[env(safe-area-inset-top)]">
      <div className="mx-auto flex h-14 max-w-screen-lg items-center gap-4 px-4">
        <Link
          href="/feed"
          className="font-heading text-2xl font-bold text-primary"
          aria-label="BandUp home"
        >
          BandUp
        </Link>

        <nav aria-label="Main" className="ml-auto hidden md:block">
          <ul className="flex items-center gap-1">
            {navItems
              .filter((item) => !item.primary)
              .map(({ href, label, icon: Icon }) => {
                const active = isActive(pathname, href);
                return (
                  <li key={href}>
                    <Link
                      href={href}
                      aria-current={active ? "page" : undefined}
                      className={cn(
                        "flex h-11 items-center gap-2 rounded-md px-3 text-sm font-medium transition-colors",
                        active
                          ? "text-primary"
                          : "text-muted-foreground hover:bg-muted hover:text-foreground",
                      )}
                    >
                      <Icon aria-hidden className="size-5" strokeWidth={active ? 2.5 : 1.75} />
                      {label}
                    </Link>
                  </li>
                );
              })}
          </ul>
        </nav>

        <Link
          href="/create"
          className="hidden h-11 items-center gap-2 rounded-md bg-primary px-4 text-sm font-semibold text-primary-foreground transition-colors hover:bg-primary-hover md:flex"
        >
          Create
        </Link>

        <Link
          href="/adverts"
          aria-label="Search adverts"
          className="ml-auto flex size-11 items-center justify-center rounded-md text-foreground hover:bg-muted md:hidden"
        >
          <Search aria-hidden className="size-6" />
        </Link>
      </div>
    </header>
  );
}
