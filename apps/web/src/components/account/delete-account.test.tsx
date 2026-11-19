import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";

import type { DeleteAccountState } from "@/lib/account";

import { DeleteAccount } from "./delete-account";

const signOut = vi.fn();
const deleteAccount = vi.fn<() => Promise<DeleteAccountState>>();

vi.mock("@clerk/nextjs", () => ({ useClerk: () => ({ signOut }) }));
vi.mock("@/app/(app)/me/actions", () => ({ deleteAccount: () => deleteAccount() }));

async function openAndConfirm(text = "DELETE") {
  const user = userEvent.setup();
  render(<DeleteAccount />);
  await user.click(screen.getByRole("button", { name: "Delete account" }));
  await user.type(screen.getByLabelText("Type DELETE to confirm"), text);
  return user;
}

describe("DeleteAccount", () => {
  beforeEach(() => {
    signOut.mockReset();
    deleteAccount.mockReset();
  });

  it("only enables the delete button once DELETE is typed", async () => {
    await openAndConfirm("delete");
    const submit = screen.getByRole("button", { name: "Delete my account" });
    expect(submit).toBeDisabled();

    const user = userEvent.setup();
    await user.clear(screen.getByLabelText("Type DELETE to confirm"));
    await user.type(screen.getByLabelText("Type DELETE to confirm"), "DELETE");
    expect(submit).toBeEnabled();
  });

  it("can be cancelled", async () => {
    const user = await openAndConfirm();

    await user.click(screen.getByRole("button", { name: "Cancel" }));

    expect(screen.getByRole("button", { name: "Delete account" })).toBeInTheDocument();
    expect(deleteAccount).not.toHaveBeenCalled();
  });

  it("signs out and goes home once the account is deleted", async () => {
    deleteAccount.mockResolvedValue({ status: "deleted" });
    const user = await openAndConfirm();

    await user.click(screen.getByRole("button", { name: "Delete my account" }));

    await waitFor(() => expect(signOut).toHaveBeenCalledWith({ redirectUrl: "/" }));
  });

  it("shows the server's message and stays signed in when it fails", async () => {
    deleteAccount.mockResolvedValue({ status: "error", message: "Your account wasn't deleted." });
    const user = await openAndConfirm();

    await user.click(screen.getByRole("button", { name: "Delete my account" }));

    expect(await screen.findByText("Your account wasn't deleted.")).toBeInTheDocument();
    expect(signOut).not.toHaveBeenCalled();
  });
});
