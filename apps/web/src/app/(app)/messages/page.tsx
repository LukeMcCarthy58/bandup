import type { Metadata } from "next";
import { MessageCircle } from "lucide-react";

import { EmptyState } from "@/components/states/empty-state";

export const metadata: Metadata = { title: "Messages" };

export default function MessagesPage() {
  return (
    <>
      <h1 className="mb-4 text-2xl">Messages</h1>
      <EmptyState
        icon={MessageCircle}
        title="No messages yet"
        description="When you reply to an advert or request to join a band, the conversation appears here."
      />
    </>
  );
}
