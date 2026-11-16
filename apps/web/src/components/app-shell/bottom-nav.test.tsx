import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { BottomNav } from "./bottom-nav";
import { isActive } from "./nav-items";

const pathname = vi.hoisted(() => ({ current: "/feed" }));
vi.mock("next/navigation", () => ({ usePathname: () => pathname.current }));

describe("BottomNav", () => {
  beforeEach(() => {
    pathname.current = "/feed";
  });

  it("shows the five tabs in order", () => {
    render(<BottomNav />);
    const labels = screen
      .getAllByRole("link")
      .map((l) => l.getAttribute("aria-label") ?? l.textContent);
    expect(labels).toEqual(["Feed", "Adverts", "Create", "Messages", "Me"]);
  });

  it("marks the current tab for screen readers", () => {
    pathname.current = "/messages/abc";
    render(<BottomNav />);
    expect(screen.getByRole("link", { name: "Messages" })).toHaveAttribute("aria-current", "page");
    expect(screen.getByRole("link", { name: "Feed" })).not.toHaveAttribute("aria-current");
  });

  it("gives the icon-only Create button an accessible name", () => {
    render(<BottomNav />);
    expect(screen.getByRole("link", { name: "Create" })).toHaveAttribute("href", "/create");
  });
});

describe("isActive", () => {
  it("matches the page and its children only", () => {
    expect(isActive("/adverts", "/adverts")).toBe(true);
    expect(isActive("/adverts/123", "/adverts")).toBe(true);
    expect(isActive("/advertsx", "/adverts")).toBe(false);
    expect(isActive("/feed", "/adverts")).toBe(false);
  });
});
