import type { LucideIcon } from "lucide-react";
import type { ReactNode } from "react";

type EmptyStateProps = {
  icon: LucideIcon;
  title: string;
  /** One line explaining the space, ending with a full stop. */
  description: string;
  action?: ReactNode;
};

/** An invitation, not an apology: say what goes here and how to start. */
export function EmptyState({ icon: Icon, title, description, action }: EmptyStateProps) {
  return (
    <div className="flex flex-col items-center rounded-lg border bg-card px-6 py-12 text-center">
      <span className="flex size-12 items-center justify-center rounded-full bg-secondary text-secondary-foreground">
        <Icon aria-hidden className="size-6" />
      </span>
      <h2 className="mt-4 text-lg">{title}</h2>
      <p className="mt-1 max-w-sm text-muted-foreground">{description}</p>
      {action ? <div className="mt-6">{action}</div> : null}
    </div>
  );
}
