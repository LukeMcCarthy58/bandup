import Link from "next/link";

import { Button } from "@/components/ui/button";

export default function NotFound() {
  return (
    <main
      id="main"
      className="mx-auto flex w-full max-w-[600px] flex-1 flex-col justify-center px-4 py-16"
    >
      <p className="font-heading text-sm font-bold text-primary">404</p>
      <h1 className="mt-2 text-3xl">That page isn&apos;t here</h1>
      <p className="mt-2 text-muted-foreground">It may have moved, or the link might be wrong.</p>
      <div className="mt-6">
        <Button asChild>
          <Link href="/feed">Go to your feed</Link>
        </Button>
      </div>
    </main>
  );
}
