import { TriangleAlert } from "lucide-react";
import type { ReactNode } from "react";

type ErrorStateProps = {
  title?: string;
  description?: string;
  action?: ReactNode;
};

/** Says what happened and what to do next. Never shows raw error details. */
export function ErrorState({
  title = "Something went wrong",
  description = "That didn't load. Check your connection and try again.",
  action,
}: ErrorStateProps) {
  return (
    <div
      role="alert"
      className="flex flex-col items-center rounded-lg border bg-card px-6 py-12 text-center"
    >
      <span className="flex size-12 items-center justify-center rounded-full bg-destructive/10 text-destructive">
        <TriangleAlert aria-hidden className="size-6" />
      </span>
      <h2 className="mt-4 text-lg">{title}</h2>
      <p className="mt-1 max-w-sm text-muted-foreground">{description}</p>
      {action ? <div className="mt-6">{action}</div> : null}
    </div>
  );
}
