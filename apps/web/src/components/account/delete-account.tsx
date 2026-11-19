"use client";

import { useClerk } from "@clerk/nextjs";
import { useActionState, useEffect, useId, useState } from "react";

import { deleteAccount } from "@/app/(app)/me/actions";
import { Button } from "@/components/ui/button";
import { FieldMessage, Input, Label } from "@/components/ui/field";
import { DELETE_CONFIRMATION, type DeleteAccountState } from "@/lib/account";

const initialState: DeleteAccountState = { status: "idle" };

/**
 * "Delete account", with a confirmation step: the person types DELETE before the button
 * works. Once the server has deleted everything, the browser signs out and goes home.
 */
export function DeleteAccount() {
  const [open, setOpen] = useState(false);
  const [confirm, setConfirm] = useState("");
  const [state, formAction, pending] = useActionState(deleteAccount, initialState);
  const { signOut } = useClerk();
  const inputId = useId();
  const messageId = useId();
  const deleted = state.status === "deleted";

  useEffect(() => {
    if (deleted) void signOut({ redirectUrl: "/" });
  }, [deleted, signOut]);

  if (!open) {
    return (
      <Button
        variant="outline"
        size="sm"
        className="text-destructive"
        onClick={() => setOpen(true)}
      >
        Delete account
      </Button>
    );
  }

  const error = state.status === "error" ? state.message : undefined;

  return (
    <form action={formAction} className="space-y-3" aria-labelledby={`${inputId}-title`}>
      <h3 id={`${inputId}-title`} className="text-base font-semibold text-destructive">
        Delete your account?
      </h3>
      <p className="text-sm text-muted-foreground">
        This permanently deletes your profile and your sign-in. It can&apos;t be undone.
      </p>
      <div>
        <Label htmlFor={inputId}>Type {DELETE_CONFIRMATION} to confirm</Label>
        <Input
          id={inputId}
          name="confirm"
          value={confirm}
          onChange={(e) => setConfirm(e.target.value)}
          autoComplete="off"
          autoCapitalize="characters"
          spellCheck={false}
          aria-invalid={error ? true : undefined}
          aria-describedby={error ? messageId : undefined}
          disabled={pending || deleted}
        />
        <FieldMessage id={messageId} error={error} />
      </div>
      <div className="flex flex-wrap gap-2">
        <Button
          type="submit"
          variant="destructive"
          disabled={confirm !== DELETE_CONFIRMATION || pending || deleted}
        >
          {pending || deleted ? "Deleting…" : "Delete my account"}
        </Button>
        <Button
          type="button"
          variant="outline"
          disabled={pending || deleted}
          onClick={() => {
            setOpen(false);
            setConfirm("");
          }}
        >
          Cancel
        </Button>
      </div>
    </form>
  );
}
