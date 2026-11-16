import type { Metadata } from "next";
import { User } from "lucide-react";

import { EmptyState } from "@/components/states/empty-state";

export const metadata: Metadata = { title: "Your profile" };

export default function MePage() {
  return (
    <>
      <h1 className="mb-4 text-2xl">Your profile</h1>
      <EmptyState
        icon={User}
        title="Set up your profile"
        description="Add your instruments, genres and town so bands near you can find you."
      />
    </>
  );
}
