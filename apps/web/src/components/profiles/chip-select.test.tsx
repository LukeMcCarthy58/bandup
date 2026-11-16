import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { useState } from "react";
import { describe, expect, it } from "vitest";

import { ChipSelect } from "./chip-select";

const options = [
  { id: 1, name: "Vocals" },
  { id: 6, name: "Bass guitar" },
  { id: 8, name: "Drums" },
];

function Harness({ max = 3, initial = [] as number[] }) {
  const [selected, setSelected] = useState(initial);
  return (
    <form data-testid="form">
      <ChipSelect
        name="instrumentIds"
        legend="What do you play?"
        options={options}
        selected={selected}
        onChange={setSelected}
        max={max}
        firstLabel="Main"
      />
    </form>
  );
}

const submitted = () =>
  new FormData(screen.getByTestId("form") as HTMLFormElement).getAll("instrumentIds");

describe("ChipSelect", () => {
  it("toggles chips, marks them pressed, and submits them in the order picked", async () => {
    render(<Harness />);

    await userEvent.click(screen.getByRole("button", { name: "Bass guitar" }));
    await userEvent.click(screen.getByRole("button", { name: "Vocals" }));

    expect(screen.getByRole("button", { name: /Bass guitar/ })).toHaveAttribute(
      "aria-pressed",
      "true",
    );
    expect(screen.getByRole("button", { name: /Bass guitar/ })).toHaveTextContent("Main");
    expect(submitted()).toEqual(["6", "1"]);

    await userEvent.click(screen.getByRole("button", { name: /Bass guitar/ }));
    expect(submitted()).toEqual(["1"]);
  });

  it("explains the limit instead of adding more", async () => {
    render(<Harness max={1} initial={[1]} />);

    await userEvent.click(screen.getByRole("button", { name: "Drums" }));

    expect(screen.getByText("You can choose up to 1.")).toBeInTheDocument();
    expect(submitted()).toEqual(["1"]);
  });
});
