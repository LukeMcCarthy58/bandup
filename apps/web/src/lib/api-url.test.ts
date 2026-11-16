import { describe, expect, it } from "vitest";

import { checkApiUrl } from "./api-url";

describe("checkApiUrl", () => {
  it("defaults to the local API", () => {
    expect(checkApiUrl(undefined)).toBe("http://localhost:5080");
    expect(checkApiUrl("  ")).toBe("http://localhost:5080");
  });

  it("keeps only the origin", () => {
    expect(checkApiUrl("https://api.bandup.example/")).toBe("https://api.bandup.example");
    expect(checkApiUrl("http://localhost:5080/api/v1")).toBe("http://localhost:5080");
  });

  it("rejects Clerk's Frontend API URL with a clear message (BAND-84)", () => {
    expect(() => checkApiUrl("https://electric-badger-7698.clerk.accounts.dev")).toThrow(
      /points at Clerk/,
    );
    expect(() => checkApiUrl("https://clerk.bandup.example")).toThrow(/points at Clerk/);
  });

  it("rejects values that aren't web URLs", () => {
    expect(() => checkApiUrl("localhost:5080")).toThrow(/must/);
    expect(() => checkApiUrl("not a url")).toThrow(/full URL/);
  });
});
