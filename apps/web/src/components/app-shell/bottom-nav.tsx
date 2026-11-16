"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

import { cn } from "@/lib/utils";

import { isActive, navItems } from "./nav-items";

/** Mobile-only tab bar, fixed to the bottom and clear of the home indicator. */
export function BottomNav() {
  const pathname = usePathname();

  return (
    <nav
      aria-label="Main"
      className="fixed inset-x-0 bottom-0 z-40 border-t bg-card pb-[env(safe-area-inset-bottom)] md:hidden"
    >
      <ul className="mx-auto flex h-16 max-w-screen-sm items-center justify-around px-2">
        {navItems.map(({ href, label, icon: Icon, primary }) => {
          const active = isActive(pathname, href);

          if (primary) {
            return (
              <li key={href}>
                <Link
                  href={href}
                  aria-label={label}
                  aria-current={active ? "page" : undefined}
                  className="flex size-11 items-center justify-center rounded-lg bg-primary text-primary-foreground transition-colors hover:bg-primary-hover"
                >
                  <Icon aria-hidden className="size-6" />
                </Link>
              </li>
            );
          }

          return (
            <li key={href}>
              <Link
                href={href}
                aria-current={active ? "page" : undefined}
                className={cn(
                  "flex min-h-11 min-w-14 flex-col items-center justify-center gap-0.5 rounded-md px-2 text-[11px] font-medium transition-colors",
                  active ? "text-primary" : "text-muted-foreground hover:text-foreground",
                )}
              >
                <Icon aria-hidden className="size-6" strokeWidth={active ? 2.5 : 1.75} />
                {label}
              </Link>
            </li>
          );
        })}
      </ul>
    </nav>
  );
}
