"use server";

import type { Schemas } from "@bandup/api-client";
import { redirect, unstable_rethrow } from "next/navigation";

import { serverApi } from "@/lib/api";
import { formToProfileRequest, toFieldErrors, type ProfileFormErrors } from "@/lib/profiles/form";

export type SaveProfileState = { errors: ProfileFormErrors };

/**
 * Saves the signed-in musician's profile through the API. Server Actions also check the
 * request's origin, so another site can't submit this form on the user's behalf.
 */
export async function saveProfile(
  _previous: SaveProfileState,
  form: FormData,
): Promise<SaveProfileState> {
  const body = formToProfileRequest(form);

  try {
    const api = await serverApi();
    const { error, response } = await api.PUT("/api/v1/me/profile", { body });

    if (response.status === 400) {
      const problem = error as Schemas["HttpValidationProblemDetails"] | undefined;
      return { errors: toFieldErrors(problem?.errors) };
    }
    if (response.status === 429) {
      return { errors: { form: "You're saving too often. Wait a moment and try again." } };
    }
    if (!response.ok) {
      return { errors: { form: "Your profile didn't save. Try again." } };
    }
  } catch (error) {
    unstable_rethrow(error);
    console.error("Saving the profile failed", error);
    return { errors: { form: "We couldn't reach BandUp's servers. Try again in a moment." } };
  }

  redirect("/me");
}
