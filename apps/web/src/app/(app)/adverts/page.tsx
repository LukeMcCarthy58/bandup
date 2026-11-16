import type { Metadata } from "next";
import Link from "next/link";
import { Megaphone } from "lucide-react";

import { EmptyState } from "@/components/states/empty-state";
import { Button } from "@/components/ui/button";

export const metadata: Metadata = { title: "Adverts" };

export default function AdvertsPage() {
  return (
    <>
      <h1 className="mb-4 text-2xl">Adverts</h1>
      <EmptyState
        icon={Megaphone}
        title="No adverts nearby yet"
        description="Bands looking for members and musicians looking for bands will show here. Widen your radius or post your own."
        action={
          <Button asChild>
            <Link href="/create">Post an advert</Link>
          </Button>
        }
      />
    </>
  );
}
