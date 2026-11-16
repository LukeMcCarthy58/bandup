import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { Music } from "lucide-react";

import { CardSkeleton } from "@/components/states/card-skeleton";
import { EmptyState } from "@/components/states/empty-state";
import { ErrorState } from "@/components/states/error-state";
import { Avatar } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";

export const metadata: Metadata = {
  title: "Style guide",
  robots: { index: false, follow: false },
};

/** Every token pair from docs/design/style-guide.md, rendered for a visual check. */
const swatches = [
  { name: "background", className: "bg-background text-foreground border" },
  { name: "card", className: "bg-card text-card-foreground border" },
  { name: "primary", className: "bg-primary text-primary-foreground" },
  { name: "primary-hover", className: "bg-primary-hover text-primary-foreground" },
  { name: "secondary", className: "bg-secondary text-secondary-foreground" },
  { name: "muted", className: "bg-muted text-muted-foreground" },
  { name: "destructive", className: "bg-destructive text-destructive-foreground" },
  { name: "success", className: "bg-success text-background" },
];

const typeScale = [
  { label: "30 · H1", className: "font-heading text-3xl font-bold" },
  { label: "24 · H2", className: "font-heading text-2xl font-bold" },
  { label: "20 · H3", className: "font-heading text-xl font-bold" },
  { label: "18 · Large", className: "text-lg" },
  { label: "16 · Body", className: "text-base" },
  { label: "14 · Small", className: "text-sm" },
  { label: "13 · Caption", className: "text-[13px] text-muted-foreground" },
];

const variants = ["default", "secondary", "outline", "ghost", "link", "destructive"] as const;

function Section({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <section className="space-y-4">
      <h2 className="text-xl">{title}</h2>
      {children}
    </section>
  );
}

export default function StyleguidePage() {
  // A design reference for local dev and preview deploys, hidden in production.
  if (process.env.VERCEL_ENV === "production") notFound();

  return (
    <main id="main" className="mx-auto w-full max-w-[600px] space-y-10 px-4 py-8">
      <header>
        <p className="font-heading text-2xl font-bold text-primary">BandUp</p>
        <h1 className="mt-1 text-3xl">Style guide</h1>
        <p className="mt-2 text-muted-foreground">
          Check this page in light and dark mode (switch your system theme) and at 360px wide. Spec:{" "}
          <code>docs/design/style-guide.md</code>.
        </p>
      </header>

      <Section title="Colours">
        <ul className="grid grid-cols-2 gap-3">
          {swatches.map(({ name, className }) => (
            <li key={name} className={`rounded-lg p-4 ${className}`}>
              <span className="font-semibold">{name}</span>
              <span className="block text-sm">Aa readable text</span>
            </li>
          ))}
        </ul>
        <p>
          Body text with a{" "}
          <a className="text-primary underline underline-offset-4" href="#">
            link
          </a>{" "}
          and <span className="text-muted-foreground">muted metadata</span>.
        </p>
      </Section>

      <Section title="Typography">
        <ul className="space-y-2 rounded-lg border bg-card p-4">
          {typeScale.map(({ label, className }) => (
            <li key={label} className="flex items-baseline gap-4">
              <span className="w-24 shrink-0 text-xs text-muted-foreground">{label}</span>
              <span className={className}>Find your band</span>
            </li>
          ))}
        </ul>
      </Section>

      <Section title="Buttons">
        <div className="flex flex-wrap gap-3">
          {variants.map((variant) => (
            <Button key={variant} variant={variant}>
              {variant}
            </Button>
          ))}
        </div>
        <div className="flex flex-wrap items-center gap-3">
          <Button size="sm">Small</Button>
          <Button>Default (44px)</Button>
          <Button size="lg">Large</Button>
          <Button size="icon" aria-label="Music">
            <Music />
          </Button>
        </div>
      </Section>

      <Section title="Avatars">
        <p className="text-muted-foreground">Musicians are circles. Bands are squircles.</p>
        <div className="flex flex-wrap items-center gap-4">
          <Avatar kind="user" name="Jade Morgan" size="lg" />
          <Avatar kind="user" name="Jade Morgan" />
          <Avatar kind="user" name="Jade Morgan" size="sm" />
          <Avatar kind="band" name="The Owls" size="lg" />
          <Avatar kind="band" name="The Owls" />
          <Avatar kind="band" name="The Owls" size="sm" />
        </div>
      </Section>

      <Section title="States">
        <CardSkeleton />
        <EmptyState
          icon={Music}
          title="No adverts nearby yet"
          description="Widen your radius or post your own."
          action={<Button>Post an advert</Button>}
        />
        <ErrorState action={<Button variant="outline">Try again</Button>} />
      </Section>
    </main>
  );
}
