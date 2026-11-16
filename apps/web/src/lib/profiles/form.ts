import type { Schemas } from "@bandup/api-client";

export const PROFILE_LIMITS = {
  displayNameMin: 2,
  displayNameMax: 60,
  bioMax: 500,
  maxInstruments: 10,
  maxGenres: 10,
} as const;

export type ProfileFormErrors = Partial<
  Record<
    | "displayName"
    | "bio"
    | "townId"
    | "level"
    | "commitment"
    | "instrumentIds"
    | "genreIds"
    | "form",
    string
  >
>;

/** Reads the profile form into the API's request shape. */
export function formToProfileRequest(form: FormData): Schemas["UpdateProfileRequest"] {
  const text = (name: string) => {
    const value = form.get(name);
    return typeof value === "string" ? value : "";
  };
  const ids = (name: string) =>
    form
      .getAll(name)
      .map((v) => Number(v))
      .filter((n) => Number.isInteger(n) && n > 0);
  const townId = Number(text("townId"));

  return {
    displayName: text("displayName").trim(),
    bio: text("bio").trim() || null,
    townId: Number.isInteger(townId) && townId > 0 ? townId : null,
    level: (text("level") || null) as Schemas["UpdateProfileRequest"]["level"],
    commitment: (text("commitment") || null) as Schemas["UpdateProfileRequest"]["commitment"],
    instrumentIds: ids("instrumentIds"),
    genreIds: ids("genreIds"),
  };
}

/**
 * Turns API validation errors into one message per form field. The API returns some keys
 * in PascalCase ("DisplayName") and some in camelCase ("townId"); both map to camelCase.
 */
export function toFieldErrors(
  errors: Record<string, string[] | undefined> | undefined,
): ProfileFormErrors {
  const result: ProfileFormErrors = {};
  for (const [key, messages] of Object.entries(errors ?? {})) {
    const field = (key.charAt(0).toLowerCase() + key.slice(1)) as keyof ProfileFormErrors;
    if (messages?.[0] && !(field in result)) result[field] = messages[0];
  }
  return result;
}

export type ProfileFormValues = {
  displayName: string;
  bio: string;
  townId: number | null;
  level: string | null;
  commitment: string | null;
  instrumentIds: number[];
  genreIds: number[];
};

/** Instant checks in the browser, mirroring the API's rules (the API still checks again). */
export function validateProfile(values: ProfileFormValues): ProfileFormErrors {
  const errors: ProfileFormErrors = {};
  const name = values.displayName.trim().replace(/\s+/g, " ");
  if (name.length < PROFILE_LIMITS.displayNameMin || name.length > PROFILE_LIMITS.displayNameMax) {
    errors.displayName = "Enter a name between 2 and 60 characters.";
  }
  if (values.bio.trim().length > PROFILE_LIMITS.bioMax) {
    errors.bio = "Keep your bio under 500 characters.";
  }
  if (!values.townId) errors.townId = "Choose your town from the list.";
  if (!values.level) errors.level = "Choose your level.";
  if (!values.commitment) errors.commitment = "Choose how much time you can give a band.";
  if (values.instrumentIds.length === 0) errors.instrumentIds = "Choose at least one instrument.";
  return errors;
}
