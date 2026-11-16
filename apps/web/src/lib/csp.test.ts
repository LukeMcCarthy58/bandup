import { describe, expect, it } from "vitest";

import { buildCsp, originsFromEnv } from "./csp";

const directive = (csp: string, name: string) =>
  csp
    .split("; ")
    .find((d) => d.startsWith(`${name} `))
    ?.slice(name.length + 1);

describe("buildCsp", () => {
  it("only allows scripts with this request's nonce", () => {
    const csp = buildCsp({ nonce: "abc123", isDev: false });
    expect(directive(csp, "script-src")).toBe("'self' 'nonce-abc123' 'strict-dynamic'");
  });

  it("never allows unsafe-eval in production", () => {
    expect(buildCsp({ nonce: "n", isDev: false })).not.toContain("unsafe-eval");
    expect(buildCsp({ nonce: "n", isDev: true })).toContain("'unsafe-eval'");
  });

  it("blocks framing, plugins and base-tag hijacking", () => {
    const csp = buildCsp({ nonce: "n", isDev: false });
    expect(directive(csp, "frame-ancestors")).toBe("'none'");
    expect(directive(csp, "object-src")).toBe("'none'");
    expect(directive(csp, "base-uri")).toBe("'self'");
  });

  it("only allows YouTube and SoundCloud embeds", () => {
    const frames = directive(buildCsp({ nonce: "n", isDev: false }), "frame-src");
    expect(frames).toBe(
      "https://www.youtube-nocookie.com https://www.youtube.com https://w.soundcloud.com",
    );
  });

  it("adds the API and image origins", () => {
    const csp = buildCsp({
      nonce: "n",
      isDev: false,
      connectSrc: ["https://api.bandup.example"],
      imgSrc: ["https://cdn.bandup.example"],
    });
    expect(directive(csp, "connect-src")).toBe("'self' https://api.bandup.example");
    expect(directive(csp, "img-src")).toBe("'self' blob: data: https://cdn.bandup.example");
  });

  it("upgrades insecure requests in production only", () => {
    expect(buildCsp({ nonce: "n", isDev: false })).toMatch(/upgrade-insecure-requests$/);
    expect(buildCsp({ nonce: "n", isDev: true })).not.toContain("upgrade-insecure-requests");
  });
});

describe("buildCsp with Clerk", () => {
  const csp = buildCsp({ nonce: "n", isDev: false, clerkHost: "happy-owl-12.clerk.accounts.dev" });

  it("allows Clerk's API, images, workers and bot protection", () => {
    expect(directive(csp, "connect-src")).toBe(
      "'self' https://happy-owl-12.clerk.accounts.dev https://*.protect.clerk.com:*",
    );
    expect(directive(csp, "img-src")).toContain("https://img.clerk.com");
    expect(directive(csp, "worker-src")).toBe("'self' blob:");
    expect(directive(csp, "frame-src")).toContain("https://challenges.cloudflare.com");
  });

  it("still requires the nonce for scripts", () => {
    expect(directive(csp, "script-src")).toMatch(/^'self' 'nonce-n' 'strict-dynamic'/);
    expect(directive(csp, "script-src")).not.toContain("unsafe-inline");
  });

  it("adds nothing for Clerk when no host is known", () => {
    expect(buildCsp({ nonce: "n", isDev: false })).not.toContain("clerk");
  });
});

describe("originsFromEnv", () => {
  it("splits and trims a comma-separated list", () => {
    expect(originsFromEnv(" https://a.test, ,https://b.test ")).toEqual([
      "https://a.test",
      "https://b.test",
    ]);
  });

  it("handles a missing value", () => {
    expect(originsFromEnv(undefined)).toEqual([]);
  });
});
