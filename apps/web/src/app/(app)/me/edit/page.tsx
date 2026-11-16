import { currentUser } from "@clerk/nextjs/server";
import type { Metadata } from "next";
import { unstable_rethrow } from "next/navigation";

import { ProfileForm } from "@/components/profiles/profile-form";
import { ErrorState } from "@/components/states/error-state";
import { serverApi } from "@/lib/api";
import { publicApiUrl } from "@/lib/api-url";

export const metadata: Metadata = { title: "Edit profile" };

async function load() {
  try {
    const api = await serverApi();
    const [profile, instruments, genres] = await Promise.all([
      api.GET("/api/v1/me/profile"),
      api.GET("/api/v1/instruments"),
      api.GET("/api/v1/genres"),
    ]);
    if (!profile.data || !instruments.data || !genres.data) return null;
    return { profile: profile.data, instruments: instruments.data, genres: genres.data };
  } catch (error) {
    unstable_rethrow(error);
    console.error("Couldn't load the profile form", error);
    return null;
  }
}

export default async function EditProfilePage() {
  const [user, loaded] = await Promise.all([currentUser(), load()]);

  if (!loaded) {
    return (
      <ErrorState
        title="We couldn't load your profile"
        description="BandUp's servers didn't respond. Try again in a moment."
      />
    );
  }

  const { profile, instruments, genres } = loaded;
  const isNew = !profile.hasProfile;

  return (
    <>
      <h1 className="text-2xl">{isNew ? "Set up your profile" : "Edit your profile"}</h1>
      <p className="mt-1 mb-6 text-muted-foreground">
        {isNew
          ? "Tell bands what you play and where you are. It takes about a minute."
          : "Changes show on your profile straight away."}
      </p>
      <ProfileForm
        apiUrl={publicApiUrl()}
        isNew={isNew}
        instruments={instruments}
        genres={genres}
        initial={{
          displayName: profile.displayName ?? user?.fullName ?? "",
          bio: profile.bio ?? "",
          townId: profile.town?.id ?? null,
          town: profile.town ?? null,
          level: profile.level ?? null,
          commitment: profile.commitment ?? null,
          instrumentIds: (profile.instruments ?? []).flatMap((i) => (i.id ? [i.id] : [])),
          genreIds: (profile.genres ?? []).flatMap((g) => (g.id ? [g.id] : [])),
        }}
      />
    </>
  );
}
