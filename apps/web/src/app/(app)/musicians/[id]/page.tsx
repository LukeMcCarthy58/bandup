import type { Metadata } from "next";
import { notFound, unstable_rethrow } from "next/navigation";

import { ProfileCard } from "@/components/profiles/profile-card";
import { ErrorState } from "@/components/states/error-state";
import { serverApi } from "@/lib/api";

export const metadata: Metadata = { title: "Musician" };

const UUID = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

export default async function MusicianPage({ params }: PageProps<"/musicians/[id]">) {
  const { id } = await params;
  if (!UUID.test(id)) notFound();

  let result;
  try {
    const api = await serverApi();
    result = await api.GET("/api/v1/users/{id}", { params: { path: { id } } });
  } catch (error) {
    unstable_rethrow(error);
    console.error("Couldn't load the musician", error);
    return (
      <ErrorState
        title="We couldn't load this profile"
        description="BandUp's servers didn't respond. Try again in a moment."
      />
    );
  }

  if (result.response.status === 404 || !result.data) notFound();

  // Follow and Message actions arrive with M10 and M6.
  return <ProfileCard profile={result.data} />;
}
