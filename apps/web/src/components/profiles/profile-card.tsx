import type { Schemas } from "@bandup/api-client";
import { MapPin } from "lucide-react";
import type { ReactNode } from "react";

import { Avatar } from "@/components/ui/avatar";
import { commitmentLabel, levelLabel } from "@/lib/profiles/labels";

const memberSince = (iso?: string) =>
  iso
    ? new Intl.DateTimeFormat("en-GB", { month: "long", year: "numeric" }).format(new Date(iso))
    : undefined;

function Chips({
  label,
  items,
  first,
}: {
  label: string;
  items: Schemas["ReferenceItem"][];
  first?: string;
}) {
  if (items.length === 0) return null;
  return (
    <section>
      <h2 className="mb-2 text-sm font-semibold text-muted-foreground">{label}</h2>
      <ul className="flex flex-wrap gap-2">
        {items.map((item, index) => (
          <li
            key={item.id}
            className="inline-flex items-center gap-1.5 rounded-full bg-secondary px-3 py-1 text-sm font-medium text-secondary-foreground"
          >
            {item.name}
            {index === 0 && first ? (
              <span className="rounded-full bg-primary px-1.5 text-[11px] text-primary-foreground">
                {first}
              </span>
            ) : null}
          </li>
        ))}
      </ul>
    </section>
  );
}

/** A musician's profile as everyone sees it. Shows the town only, never an address. */
export function ProfileCard({
  profile,
  photoUrl,
  actions,
}: {
  profile: Schemas["ProfileView"];
  photoUrl?: string;
  actions?: ReactNode;
}) {
  const name = profile.displayName ?? "Musician";
  const facts = [levelLabel(profile.level), commitmentLabel(profile.commitment)].filter(Boolean);

  return (
    <article className="space-y-5 rounded-lg border bg-card p-4 sm:p-6">
      <header className="flex items-start gap-4">
        <Avatar kind="user" name={name} src={photoUrl} size="lg" />
        <div className="min-w-0 flex-1">
          <h1 className="truncate text-2xl">{name}</h1>
          {profile.town ? (
            <p className="flex items-center gap-1 text-muted-foreground">
              <MapPin aria-hidden className="size-4 shrink-0" />
              <span className="truncate">
                {[profile.town.name, profile.town.region].filter(Boolean).join(", ")}
              </span>
            </p>
          ) : null}
          {facts.length > 0 ? (
            <p className="text-sm text-muted-foreground">{facts.join(" · ")}</p>
          ) : null}
        </div>
      </header>

      {actions ? <div className="flex flex-wrap gap-2">{actions}</div> : null}

      <Chips label="Plays" items={profile.instruments ?? []} first="Main" />
      <Chips label="Into" items={profile.genres ?? []} />

      {profile.bio ? (
        <section>
          <h2 className="mb-1 text-sm font-semibold text-muted-foreground">About</h2>
          <p className="whitespace-pre-line">{profile.bio}</p>
        </section>
      ) : null}

      {memberSince(profile.memberSince) ? (
        <p className="text-sm text-muted-foreground">
          Member since {memberSince(profile.memberSince)}
        </p>
      ) : null}
    </article>
  );
}
