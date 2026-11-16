import { cn } from "@/lib/utils";

function Bar({ className }: { className?: string }) {
  return <div className={cn("animate-pulse rounded bg-muted", className)} />;
}

/** Placeholder shaped like a feed card, so the layout doesn't jump when data arrives. */
export function CardSkeleton() {
  return (
    <div className="rounded-lg border bg-card p-4" aria-hidden>
      <div className="flex items-center gap-3">
        <Bar className="size-10 rounded-full" />
        <div className="flex-1 space-y-2">
          <Bar className="h-3.5 w-1/3" />
          <Bar className="h-3 w-1/4" />
        </div>
      </div>
      <div className="mt-4 space-y-2">
        <Bar className="h-3.5 w-full" />
        <Bar className="h-3.5 w-5/6" />
      </div>
    </div>
  );
}

export function FeedSkeleton({ count = 3 }: { count?: number }) {
  return (
    <div role="status" aria-label="Loading" className="space-y-3">
      {Array.from({ length: count }, (_, i) => (
        <CardSkeleton key={i} />
      ))}
    </div>
  );
}
