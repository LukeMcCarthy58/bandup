import { SignOutButton } from "@clerk/nextjs";
import { currentUser } from "@clerk/nextjs/server";
import type { Metadata } from "next";
import { User } from "lucide-react";

import { EmptyState } from "@/components/states/empty-state";
import { ErrorState } from "@/components/states/error-state";
import { Avatar } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { getMe, type Me } from "@/lib/api";

export const metadata: Metadata = { title: "Your profile" };

const memberSince = (iso: string) =>
  new Intl.DateTimeFormat("en-GB", { month: "long", year: "numeric" }).format(new Date(iso));

export default async function MePage() {
  const user = await currentUser();
  const name = user?.fullName ?? user?.username ?? "You";
  const email = user?.primaryEmailAddress?.emailAddress;

  let account: Me | null = null;
  try {
    account = await getMe();
  } catch (error) {
    console.error("Couldn't load the BandUp account", error);
  }

  return (
    <>
      <h1 className="mb-4 text-2xl">Your profile</h1>

      <section className="mb-4 flex items-center gap-4 rounded-lg border bg-card p-4">
        <Avatar kind="user" name={name} src={user?.imageUrl} size="lg" />
        <div className="min-w-0 flex-1">
          <p className="truncate font-semibold">{name}</p>
          {email ? <p className="truncate text-sm text-muted-foreground">{email}</p> : null}
          {account ? (
            <p className="text-sm text-muted-foreground">
              Member since {memberSince(account.createdAt)}
            </p>
          ) : null}
        </div>
        <SignOutButton>
          <Button variant="outline" size="sm">
            Sign out
          </Button>
        </SignOutButton>
      </section>

      {account ? (
        <EmptyState
          icon={User}
          title="Set up your profile"
          description="Add your instruments, genres and town so bands near you can find you."
        />
      ) : (
        <ErrorState
          title="We couldn't load your account"
          description="BandUp's servers didn't respond. Try again in a moment."
        />
      )}
    </>
  );
}
