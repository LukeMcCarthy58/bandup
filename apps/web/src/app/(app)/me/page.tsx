import { SignOutButton } from "@clerk/nextjs";
import { currentUser } from "@clerk/nextjs/server";
import type { Schemas } from "@bandup/api-client";
import type { Metadata } from "next";
import Link from "next/link";
import { unstable_rethrow } from "next/navigation";
import { UserRoundPen } from "lucide-react";

import { DeleteAccount } from "@/components/account/delete-account";
import { ProfileCard } from "@/components/profiles/profile-card";
import { EmptyState } from "@/components/states/empty-state";
import { ErrorState } from "@/components/states/error-state";
import { Button } from "@/components/ui/button";
import { serverApi } from "@/lib/api";

export const metadata: Metadata = { title: "Your profile" };

async function loadProfile(): Promise<Schemas["ProfileView"] | null> {
  try {
    const api = await serverApi();
    const { data } = await api.GET("/api/v1/me/profile");
    return data ?? null;
  } catch (error) {
    unstable_rethrow(error);
    console.error("Couldn't load the profile", error);
    return null;
  }
}

export default async function MePage() {
  const [user, profile] = await Promise.all([currentUser(), loadProfile()]);
  const email = user?.primaryEmailAddress?.emailAddress;

  return (
    <div className="space-y-4">
      {!profile ? (
        <>
          <h1 className="text-2xl">Your profile</h1>
          <ErrorState
            title="We couldn't load your profile"
            description="BandUp's servers didn't respond. Try again in a moment."
          />
        </>
      ) : profile.hasProfile ? (
        <ProfileCard
          profile={profile}
          photoUrl={user?.imageUrl}
          actions={
            <>
              <Button asChild size="sm">
                <Link href="/me/edit">Edit profile</Link>
              </Button>
              <Button asChild size="sm" variant="outline">
                <Link href={`/musicians/${profile.userId}`}>See it as others do</Link>
              </Button>
            </>
          }
        />
      ) : (
        <>
          <h1 className="text-2xl">Your profile</h1>
          <EmptyState
            icon={UserRoundPen}
            title="Set up your profile"
            description="Add your instruments, genres and town so bands near you can find you."
            action={
              <Button asChild>
                <Link href="/me/edit">Set up profile</Link>
              </Button>
            }
          />
        </>
      )}

      <section className="space-y-4 rounded-lg border bg-card p-4">
        <div className="flex items-center gap-3">
          <div className="min-w-0 flex-1">
            <h2 className="text-base">Account</h2>
            {email ? <p className="truncate text-sm text-muted-foreground">{email}</p> : null}
          </div>
          <SignOutButton>
            <Button variant="outline" size="sm">
              Sign out
            </Button>
          </SignOutButton>
        </div>
        <div className="border-t pt-4">
          <DeleteAccount />
        </div>
      </section>
    </div>
  );
}
