import type { Metadata } from "next";
import { Home } from "lucide-react";

import { EmptyState } from "@/components/states/empty-state";

export const metadata: Metadata = { title: "Feed" };

export default function FeedPage() {
  return (
    <>
      <h1 className="sr-only">Feed</h1>
      <EmptyState
        icon={Home}
        title="Your feed starts here"
        description="Posts from musicians and bands near you, and the ones you follow, will show up here."
      />
    </>
  );
}
