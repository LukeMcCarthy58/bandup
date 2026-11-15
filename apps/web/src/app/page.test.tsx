import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import Home from "./page";

describe("Home page", () => {
  it("shows the headline", () => {
    render(<Home />);
    expect(screen.getByRole("heading", { level: 1, name: "Find your band" })).toBeInTheDocument();
  });

  it("links to sign up", () => {
    render(<Home />);
    expect(screen.getByRole("link", { name: "Join BandUp" })).toHaveAttribute("href", "/sign-up");
  });
});
