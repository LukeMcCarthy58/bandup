import { describe, expect, it } from "vitest";

import { clerkFrontendApiHost } from "./clerk";

const keyFor = (host: string, env = "test") => `pk_${env}_${btoa(`${host}$`)}`;

describe("clerkFrontendApiHost", () => {
  it("decodes the host from a publishable key", () => {
    expect(clerkFrontendApiHost(keyFor("happy-owl-12.clerk.accounts.dev"))).toBe(
      "happy-owl-12.clerk.accounts.dev",
    );
    expect(clerkFrontendApiHost(keyFor("clerk.bandup.app", "live"))).toBe("clerk.bandup.app");
  });

  it("returns undefined for missing or malformed keys", () => {
    expect(clerkFrontendApiHost(undefined)).toBeUndefined();
    expect(clerkFrontendApiHost("pk_test_")).toBeUndefined();
    expect(clerkFrontendApiHost("pk_test_!!!")).toBeUndefined();
    expect(clerkFrontendApiHost(`pk_test_${btoa("evil.com; script-src *$")}`)).toBeUndefined();
  });
});
