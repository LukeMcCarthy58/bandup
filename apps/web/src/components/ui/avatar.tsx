import { cn } from "@/lib/utils";

const sizes = {
  sm: "size-8 text-xs",
  md: "size-10 text-sm",
  lg: "size-16 text-lg",
} as const;

type AvatarProps = {
  name: string;
  /** Musicians are circles, bands are squircles, so you can tell them apart (ADR 0007). */
  kind: "user" | "band";
  src?: string | null;
  size?: keyof typeof sizes;
  className?: string;
};

export function initials(name: string): string {
  const parts = name.trim().split(/\s+/).filter(Boolean);
  if (parts.length === 0) return "?";
  const first = parts[0][0];
  const last = parts.length > 1 ? parts[parts.length - 1][0] : "";
  return (first + last).toUpperCase();
}

export function Avatar({ name, kind, src, size = "md", className }: AvatarProps) {
  const shape = kind === "band" ? "rounded-[28%]" : "rounded-full";

  return (
    <span
      data-kind={kind}
      className={cn(
        "inline-flex shrink-0 items-center justify-center overflow-hidden bg-secondary font-semibold text-secondary-foreground",
        sizes[size],
        shape,
        className,
      )}
    >
      {src ? (
        // Plain img for now; switched to next/image with the CDN in the Images epic.
        // eslint-disable-next-line @next/next/no-img-element
        <img src={src} alt={name} className="size-full object-cover" />
      ) : (
        <span aria-label={name} role="img">
          {initials(name)}
        </span>
      )}
    </span>
  );
}
