"use client";

import type { Schemas } from "@bandup/api-client";
import { useActionState, useId, useState } from "react";

import { saveProfile, type SaveProfileState } from "@/app/(app)/me/edit/actions";
import { Button } from "@/components/ui/button";
import { FieldMessage, Input, Label, Textarea } from "@/components/ui/field";
import {
  PROFILE_LIMITS,
  validateProfile,
  type ProfileFormErrors,
  type ProfileFormValues,
} from "@/lib/profiles/form";
import { commitmentOptions, levelOptions } from "@/lib/profiles/labels";

import { ChipSelect } from "./chip-select";
import { OptionCards } from "./option-cards";
import { TownPicker, type Town } from "./town-picker";

type ReferenceItem = Schemas["ReferenceItem"];

type ProfileFormProps = {
  apiUrl: string;
  initial: ProfileFormValues & { town: Town | null };
  instruments: ReferenceItem[];
  genres: ReferenceItem[];
  isNew: boolean;
};

const toOptions = (items: ReferenceItem[]) =>
  items.flatMap((i) => (i.id && i.name ? [{ id: i.id, name: i.name }] : []));

/**
 * The profile form. Fields are controlled, so nothing is lost if the server rejects a value;
 * the browser checks first for instant feedback, then the API checks again.
 */
export function ProfileForm({ apiUrl, initial, instruments, genres, isNew }: ProfileFormProps) {
  const ids = { name: useId(), nameMsg: useId(), bio: useId(), bioMsg: useId() };
  const [values, setValues] = useState<ProfileFormValues>(initial);
  const [town, setTown] = useState<Town | null>(initial.town);
  const [clientErrors, setClientErrors] = useState<ProfileFormErrors>({});
  const [state, formAction, pending] = useActionState<SaveProfileState, FormData>(saveProfile, {
    errors: {},
  });

  const errors: ProfileFormErrors = { ...state.errors, ...clientErrors };
  const set = <K extends keyof ProfileFormValues>(key: K, value: ProfileFormValues[K]) => {
    setValues((v) => ({ ...v, [key]: value }));
    setClientErrors((e) => ({ ...e, [key]: undefined }));
  };

  function submit(form: FormData) {
    const problems = validateProfile(values);
    setClientErrors(problems);
    if (Object.values(problems).some(Boolean)) return;
    formAction(form);
  }

  return (
    <form action={submit} noValidate className="space-y-6">
      {errors.form ? (
        <p role="alert" className="rounded-md border border-destructive p-3 text-destructive">
          {errors.form}
        </p>
      ) : null}

      <div>
        <Label htmlFor={ids.name}>Name</Label>
        <Input
          id={ids.name}
          name="displayName"
          autoComplete="name"
          maxLength={PROFILE_LIMITS.displayNameMax}
          value={values.displayName}
          onChange={(e) => set("displayName", e.target.value)}
          aria-invalid={!!errors.displayName}
          aria-describedby={ids.nameMsg}
        />
        <FieldMessage id={ids.nameMsg} error={errors.displayName}>
          How you&apos;ll appear to bands and other musicians.
        </FieldMessage>
      </div>

      <TownPicker
        apiUrl={apiUrl}
        value={town}
        onChange={(t) => {
          setTown(t);
          set("townId", t?.id ?? null);
        }}
        error={errors.townId}
      />

      <ChipSelect
        name="instrumentIds"
        legend="What do you play?"
        hint="Pick your main instrument first."
        options={toOptions(instruments)}
        selected={values.instrumentIds}
        onChange={(v) => set("instrumentIds", v)}
        max={PROFILE_LIMITS.maxInstruments}
        firstLabel="Main"
        error={errors.instrumentIds}
      />

      <ChipSelect
        name="genreIds"
        legend="Genres you're into"
        hint="Optional, up to 10."
        options={toOptions(genres)}
        selected={values.genreIds}
        onChange={(v) => set("genreIds", v)}
        max={PROFILE_LIMITS.maxGenres}
        error={errors.genreIds}
      />

      <OptionCards
        name="level"
        legend="Your level"
        options={levelOptions}
        value={values.level as (typeof levelOptions)[number]["value"] | null}
        onChange={(v) => set("level", v)}
        error={errors.level}
      />

      <OptionCards
        name="commitment"
        legend="How much time can you give a band?"
        options={commitmentOptions}
        value={values.commitment as (typeof commitmentOptions)[number]["value"] | null}
        onChange={(v) => set("commitment", v)}
        error={errors.commitment}
      />

      <div>
        <Label htmlFor={ids.bio}>About you</Label>
        <Textarea
          id={ids.bio}
          name="bio"
          maxLength={PROFILE_LIMITS.bioMax}
          placeholder="Influences, gear, what you're looking for…"
          value={values.bio}
          onChange={(e) => set("bio", e.target.value)}
          aria-invalid={!!errors.bio}
          aria-describedby={ids.bioMsg}
        />
        <FieldMessage id={ids.bioMsg} error={errors.bio}>
          Optional. {PROFILE_LIMITS.bioMax - values.bio.length} characters left.
        </FieldMessage>
      </div>

      <div className="flex flex-col-reverse gap-3 sm:flex-row sm:justify-end">
        <Button type="submit" disabled={pending} className="sm:min-w-40">
          {pending ? "Saving…" : isNew ? "Create profile" : "Save changes"}
        </Button>
      </div>
    </form>
  );
}
