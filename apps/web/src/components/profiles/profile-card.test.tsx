import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { ProfileCard } from "./profile-card";

describe("ProfileCard", () => {
  it("shows the musician's details, main instrument and town (never more)", () => {
    render(
      <ProfileCard
        profile={{
          userId: "0196a5c4-0000-7000-8000-000000000001",
          hasProfile: true,
          displayName: "Jade Morgan",
          bio: "Bassist into tight indie grooves.",
          town: { id: 3, name: "Manchester", region: "Greater Manchester", country: "England" },
          level: "Intermediate",
          commitment: "Regular",
          instruments: [
            { id: 6, name: "Bass guitar", slug: "bass-guitar" },
            { id: 1, name: "Vocals", slug: "vocals" },
          ],
          genres: [{ id: 2, name: "Indie", slug: "indie" }],
          memberSince: "2026-11-16T16:46:54Z",
        }}
      />,
    );

    expect(screen.getByRole("heading", { level: 1, name: "Jade Morgan" })).toBeInTheDocument();
    expect(screen.getByText("Manchester, Greater Manchester")).toBeInTheDocument();
    expect(screen.getByText("Intermediate · Regular")).toBeInTheDocument();
    expect(screen.getByText("Bass guitar").parentElement).toHaveTextContent("Main");
    expect(screen.getByText("Member since November 2026")).toBeInTheDocument();
  });

  it("leaves out sections that are empty", () => {
    render(
      <ProfileCard
        profile={{
          userId: "6f1c3c1e-3d1a-4a5b-9a43-0d3f1f6f2b11",
          hasProfile: true,
          displayName: "Sam",
          bio: null,
          town: null,
          level: null,
          commitment: null,
          instruments: [],
          genres: [],
          memberSince: "2026-11-16T10:00:00Z",
        }}
      />,
    );

    expect(screen.queryByText("About")).not.toBeInTheDocument();
    expect(screen.queryByText("Into")).not.toBeInTheDocument();
  });
});
