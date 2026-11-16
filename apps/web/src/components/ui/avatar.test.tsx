import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { Avatar, initials } from "./avatar";

describe("Avatar", () => {
  it("draws users as circles and bands as squircles", () => {
    const { container } = render(
      <>
        <Avatar kind="user" name="Jade Morgan" />
        <Avatar kind="band" name="The Owls" />
      </>,
    );
    expect(container.querySelector('[data-kind="user"]')).toHaveClass("rounded-full");
    expect(container.querySelector('[data-kind="band"]')).toHaveClass("rounded-[28%]");
  });

  it("falls back to labelled initials without a photo", () => {
    render(<Avatar kind="user" name="Jade Morgan" />);
    expect(screen.getByRole("img", { name: "Jade Morgan" })).toHaveTextContent("JM");
  });

  it("uses the photo with alt text when there is one", () => {
    render(<Avatar kind="band" name="The Owls" src="/owls.webp" />);
    expect(screen.getByRole("img", { name: "The Owls" })).toHaveAttribute("src", "/owls.webp");
  });
});

describe("initials", () => {
  it.each([
    ["Jade Morgan", "JM"],
    ["The Arctic Owls", "TO"],
    ["Cher", "C"],
    ["  ", "?"],
  ])("%s → %s", (name, expected) => {
    expect(initials(name)).toBe(expected);
  });
});
