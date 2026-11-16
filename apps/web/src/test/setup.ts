import "@testing-library/jest-dom/vitest";
import { cleanup } from "@testing-library/react";
import { afterEach, vi } from "vitest";

// Clerk's UI components need a running Clerk instance; unit tests render simple stand-ins.
vi.mock("@clerk/nextjs", () => ({
  UserButton: () => null,
  SignOutButton: ({ children }: { children: React.ReactNode }) => children,
}));

afterEach(() => cleanup());
