import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import Home from "./page";

const clerk = vi.hoisted(() => ({ userId: null as string | null }));
vi.mock("@clerk/nextjs/server", () => ({ auth: async () => ({ userId: clerk.userId }) }));

const redirect = vi.hoisted(() => vi.fn());
vi.mock("next/navigation", () => ({ redirect }));

describe("Home page", () => {
  beforeEach(() => {
    clerk.userId = null;
    redirect.mockReset();
  });

  it("invites signed-out visitors to join or sign in", async () => {
    render(await Home());

    expect(screen.getByRole("heading", { level: 1, name: "Find your band" })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Join BandUp" })).toHaveAttribute("href", "/sign-up");
    expect(screen.getByRole("link", { name: "I already have an account" })).toHaveAttribute(
      "href",
      "/sign-in",
    );
  });

  it("sends signed-in users to their feed", async () => {
    clerk.userId = "user_123";
    await Home();
    expect(redirect).toHaveBeenCalledWith("/feed");
  });
});
