import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";

import { Button } from "./button";

describe("Button", () => {
  it("calls onClick when pressed", async () => {
    const onClick = vi.fn();
    render(<Button onClick={onClick}>Post</Button>);
    await userEvent.click(screen.getByRole("button", { name: "Post" }));
    expect(onClick).toHaveBeenCalledOnce();
  });

  it("meets the 44px touch target by default", () => {
    render(<Button>Post</Button>);
    expect(screen.getByRole("button")).toHaveClass("h-11");
  });
});
