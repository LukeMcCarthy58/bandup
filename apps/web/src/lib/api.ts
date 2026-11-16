import "server-only";

import { auth } from "@clerk/nextjs/server";

/**
 * Server-side client for the BandUp API. Sends the signed-in user's Clerk session
 * token, which the API validates (ADR 0010). Server-only, so tokens never reach
 * client bundles. Replaced by the generated client in BAND-23.
 */
const baseUrl = process.env.API_URL ?? process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5080";

export class ApiError extends Error {
  constructor(
    readonly status: number,
    message: string,
  ) {
    super(message);
    this.name = "ApiError";
  }
}

export async function apiFetch<T>(path: string, init: RequestInit = {}): Promise<T> {
  const { getToken } = await auth();
  const token = await getToken();

  const headers = new Headers(init.headers);
  headers.set("Accept", "application/json");
  if (token) headers.set("Authorization", `Bearer ${token}`);

  const response = await fetch(`${baseUrl}/api/v1${path}`, { ...init, headers, cache: "no-store" });
  if (!response.ok) {
    throw new ApiError(response.status, `BandUp API ${path} returned ${response.status}`);
  }

  return (await response.json()) as T;
}

/** The signed-in user's BandUp account. Created by the API on first call. */
export type Me = {
  id: string;
  createdAt: string;
};

export const getMe = () => apiFetch<Me>("/me");
