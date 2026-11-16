"use client";

import { useEffect } from "react";

import { ErrorState } from "@/components/states/error-state";
import { Button } from "@/components/ui/button";

export default function AppError({ error, retry }: { error: unknown; retry: () => void }) {
  useEffect(() => {
    // Replaced with real error reporting when we deploy.
    console.error(error);
  }, [error]);

  return (
    <ErrorState
      action={
        <Button variant="outline" onClick={() => retry()}>
          Try again
        </Button>
      }
    />
  );
}
