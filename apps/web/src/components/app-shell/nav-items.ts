import { Home, Megaphone, MessageCircle, Plus, User, type LucideIcon } from "lucide-react";

export type NavItem = {
  href: string;
  label: string;
  icon: LucideIcon;
  /** The centre "Create" action is styled as a button, not a tab. */
  primary?: boolean;
};

/** Order matches the mobile bottom tab bar (ADR 0007). */
export const navItems: NavItem[] = [
  { href: "/feed", label: "Feed", icon: Home },
  { href: "/adverts", label: "Adverts", icon: Megaphone },
  { href: "/create", label: "Create", icon: Plus, primary: true },
  { href: "/messages", label: "Messages", icon: MessageCircle },
  { href: "/me", label: "Me", icon: User },
];

/** A tab is active on its own page and any page beneath it. */
export function isActive(pathname: string, href: string): boolean {
  return pathname === href || pathname.startsWith(`${href}/`);
}
