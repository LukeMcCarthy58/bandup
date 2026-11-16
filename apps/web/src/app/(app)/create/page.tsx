import type { Metadata } from "next";
import { Megaphone, PenLine } from "lucide-react";

export const metadata: Metadata = { title: "Create" };

const options = [
  {
    icon: PenLine,
    title: "Write a post",
    description: "Share news, a gig, a photo, or a YouTube or SoundCloud link.",
  },
  {
    icon: Megaphone,
    title: "Post an advert",
    description: "Look for a band to join, or a new member for your band.",
  },
];

export default function CreatePage() {
  return (
    <>
      <h1 className="mb-4 text-2xl">Create</h1>
      <ul className="space-y-3">
        {options.map(({ icon: Icon, title, description }) => (
          <li key={title} className="flex gap-4 rounded-lg border bg-card p-4">
            <span className="flex size-11 shrink-0 items-center justify-center rounded-lg bg-secondary text-secondary-foreground">
              <Icon aria-hidden className="size-5" />
            </span>
            <div>
              <h2 className="text-base">{title}</h2>
              <p className="text-sm text-muted-foreground">{description}</p>
            </div>
          </li>
        ))}
      </ul>
    </>
  );
}
