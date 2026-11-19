"use server";

import { auth, clerkClient } from "@clerk/nextjs/server";
import { unstable_rethrow } from "next/navigation";

import { DELETE_CONFIRMATION, type DeleteAccountState } from "@/lib/account";
import { serverApi } from "@/lib/api";

/**
 * Permanently deletes the signed-in user's account (ADR 0014):
 * 1. BandUp's data, through the API (only ever the caller's own account).
 * 2. Their Clerk user, so they can't sign back in to an empty account. The secret key
 *    stays in the web server; the API never holds it.
 * The browser then signs out. Server Actions check the request's origin, so another
 * site can't trigger this.
 */
export async function deleteAccount(
  _previous: DeleteAccountState,
  form: FormData,
): Promise<DeleteAccountState> {
  if (form.get("confirm") !== DELETE_CONFIRMATION) {
    return { status: "error", message: `Type ${DELETE_CONFIRMATION} to confirm.` };
  }

  const { userId } = await auth();
  if (!userId) {
    return { status: "error", message: "You're signed out. Sign in and try again." };
  }

  try {
    const api = await serverApi();
    const { response } = await api.DELETE("/api/v1/me");
    if (response.status === 429) {
      return { status: "error", message: "Too many attempts. Wait a minute and try again." };
    }
    if (!response.ok) {
      return { status: "error", message: "Your account wasn't deleted. Try again." };
    }
  } catch (error) {
    unstable_rethrow(error);
    console.error("Deleting the BandUp account failed", error);
    return {
      status: "error",
      message: "We couldn't reach BandUp's servers. Your account wasn't deleted.",
    };
  }

  try {
    const clerk = await clerkClient();
    await clerk.users.deleteUser(userId);
  } catch (error) {
    // BandUp's data is already gone. Trying again is safe: the API delete does nothing the
    // second time, and this step runs again.
    console.error("Deleting the Clerk user failed", error);
    return {
      status: "error",
      message: "Your BandUp data is deleted, but we couldn't close your sign-in. Try again.",
    };
  }

  return { status: "deleted" };
}
