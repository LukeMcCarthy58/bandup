import { render, screen } from "@testing-library/react";
import { Music } from "lucide-react";
import { describe, expect, it } from "vitest";

import { FeedSkeleton } from "./card-skeleton";
import { EmptyState } from "./empty-state";
import { ErrorState } from "./error-state";

describe("states", () => {
  it("EmptyState shows its title, description and action", () => {
    render(
      <EmptyState
        icon={Music}
        title="No adverts nearby yet"
        description="Post your own."
        action={<button>Post an advert</button>}
      />,
    );
    expect(screen.getByRole("heading", { name: "No adverts nearby yet" })).toBeInTheDocument();
    expect(screen.getByText("Post your own.")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Post an advert" })).toBeInTheDocument();
  });

  it("ErrorState is announced to screen readers", () => {
    render(<ErrorState />);
    expect(screen.getByRole("alert")).toHaveTextContent("Something went wrong");
  });

  it("FeedSkeleton is announced as loading", () => {
    render(<FeedSkeleton count={2} />);
    expect(screen.getByRole("status", { name: "Loading" })).toBeInTheDocument();
  });
});
