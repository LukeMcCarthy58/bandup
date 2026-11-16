import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { useState } from "react";
import { afterEach, describe, expect, it, vi } from "vitest";

import { TownPicker, type Town } from "./town-picker";

const manchester: Town = {
  id: 3,
  name: "Manchester",
  region: "Greater Manchester",
  country: "England",
};

function Harness() {
  const [town, setTown] = useState<Town | null>(null);
  return (
    <form data-testid="form">
      <TownPicker apiUrl="http://api.test" value={town} onChange={setTown} />
    </form>
  );
}

afterEach(() => vi.unstubAllGlobals());

describe("TownPicker", () => {
  it("searches as you type and picks a town with the keyboard", async () => {
    const fetchMock = vi.fn(async () => new Response(JSON.stringify([manchester])));
    vi.stubGlobal("fetch", fetchMock);
    render(<Harness />);

    const input = screen.getByRole("combobox", { name: "Town or area" });
    await userEvent.type(input, "man");

    await screen.findByRole("option", { name: /Manchester/ });
    expect(fetchMock).toHaveBeenCalledWith(
      "http://api.test/api/v1/towns?query=man&limit=8",
      expect.anything(),
    );

    await userEvent.keyboard("{ArrowDown}{Enter}");

    expect(input).toHaveValue("Manchester, Greater Manchester");
    const form = new FormData(screen.getByTestId("form") as HTMLFormElement);
    expect(form.get("townId")).toBe("3");
  });

  it("doesn't search for a single character", async () => {
    const fetchMock = vi.fn();
    vi.stubGlobal("fetch", fetchMock);
    render(<Harness />);

    await userEvent.type(screen.getByRole("combobox"), "m");
    await new Promise((r) => setTimeout(r, 300));

    await waitFor(() => expect(fetchMock).not.toHaveBeenCalled());
  });
});
