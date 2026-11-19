/** What the person must type to confirm deleting their account. Checked on the server too. */
export const DELETE_CONFIRMATION = "DELETE";

export type DeleteAccountState =
  { status: "idle" } | { status: "deleted" } | { status: "error"; message: string };
