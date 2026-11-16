import type { Schemas } from "@bandup/api-client";

// The API marks these enums nullable (the fields are optional), so strip null for the choices.
export type ExperienceLevel = NonNullable<Schemas["ExperienceLevel"]>;
export type Commitment = NonNullable<Schemas["Commitment"]>;

export const levelOptions: { value: ExperienceLevel; label: string; hint: string }[] = [
  { value: "Beginner", label: "Beginner", hint: "Learning the ropes" },
  { value: "Intermediate", label: "Intermediate", hint: "Comfortable playing with others" },
  { value: "Advanced", label: "Advanced", hint: "Experienced, gigged plenty" },
  { value: "Professional", label: "Professional", hint: "Music is (or was) the day job" },
];

export const commitmentOptions: { value: Commitment; label: string; hint: string }[] = [
  { value: "Casual", label: "Casual", hint: "Jams and the odd gig" },
  { value: "Regular", label: "Regular", hint: "Weekly rehearsals and regular gigs" },
  { value: "Serious", label: "Serious", hint: "Touring, recording, aiming high" },
];

export const levelLabel = (value?: ExperienceLevel | null) =>
  levelOptions.find((o) => o.value === value)?.label;

export const commitmentLabel = (value?: Commitment | null) =>
  commitmentOptions.find((o) => o.value === value)?.label;
