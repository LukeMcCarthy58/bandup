import * as React from "react";

import { cn } from "@/lib/utils";

const control =
  "w-full rounded-md border border-input bg-card px-3 text-base text-foreground placeholder:text-muted-foreground focus-visible:outline-2 focus-visible:outline-offset-1 focus-visible:outline-ring aria-invalid:border-destructive";

export function Input({ className, ...props }: React.ComponentProps<"input">) {
  return <input className={cn(control, "h-11", className)} {...props} />;
}

export function Textarea({ className, ...props }: React.ComponentProps<"textarea">) {
  return <textarea className={cn(control, "min-h-28 py-2", className)} {...props} />;
}

export function Label({ className, ...props }: React.ComponentProps<"label">) {
  return <label className={cn("mb-1.5 block text-sm font-semibold", className)} {...props} />;
}

/** Field help or error text, linked to its control with aria-describedby. */
export function FieldMessage({
  id,
  error,
  children,
}: {
  id: string;
  error?: string;
  children?: React.ReactNode;
}) {
  if (error) {
    return (
      <p id={id} className="mt-1.5 text-sm font-medium text-destructive">
        {error}
      </p>
    );
  }
  return children ? (
    <p id={id} className="mt-1.5 text-sm text-muted-foreground">
      {children}
    </p>
  ) : null;
}
