import { auth } from "@clerk/nextjs/server";
import Link from "next/link";
import { redirect } from "next/navigation";

import { Button } from "@/components/ui/button";

export default async function Home() {
  // Signed-in users go straight to their feed.
  const { userId } = await auth();
  if (userId) redirect("/feed");

  return (
    <>
      <header className="border-b bg-card">
        <div className="mx-auto flex h-14 max-w-screen-md items-center justify-between px-4">
          <span className="font-heading text-2xl font-bold text-primary">BandUp</span>
          <Button asChild variant="ghost">
            <Link href="/sign-in">Sign in</Link>
          </Button>
        </div>
      </header>

      <main id="main" className="mx-auto w-full max-w-screen-md flex-1 px-4 py-8 sm:py-16">
        <section className="rounded-lg border bg-card p-6 sm:p-10">
          <h1 className="text-3xl sm:text-4xl">Find your band</h1>
          <p className="mt-3 max-w-prose text-muted-foreground">
            Musicians find bands, and bands find members. Share what you&apos;re playing with people
            nearby.
          </p>
          <div className="mt-6 flex flex-col gap-3 sm:flex-row">
            <Button asChild>
              <Link href="/sign-up">Join BandUp</Link>
            </Button>
            <Button asChild variant="outline">
              <Link href="/sign-in">I already have an account</Link>
            </Button>
          </div>
        </section>
      </main>
    </>
  );
}
