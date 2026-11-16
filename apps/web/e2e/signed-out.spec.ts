import { expect, test } from "@playwright/test";

import { trackConsoleErrors } from "./helpers";

test.use({ storageState: { cookies: [], origins: [] } });

test("landing page invites visitors to join or sign in", async ({ page }) => {
  const errors = trackConsoleErrors(page);
  await page.goto("/");

  await expect(page.getByRole("heading", { level: 1, name: "Find your band" })).toBeVisible();
  await expect(page.getByRole("link", { name: "Join BandUp" })).toHaveAttribute("href", "/sign-up");
  expect(errors).toEqual([]);
});

test("app pages send signed-out visitors to sign in, then back", async ({ page }) => {
  await page.goto("/messages");

  await expect(page).toHaveURL(/\/sign-in\?redirect_url=.*%2Fmessages/);
});

test("the sign-in page loads Clerk without CSP violations", async ({ page }) => {
  const errors = trackConsoleErrors(page);
  await page.goto("/sign-in");

  await expect(page.getByRole("link", { name: "BandUp" })).toBeVisible();
  // Clerk's form renders an email field once its script has loaded from its own domain.
  await expect(page.getByLabel(/email/i).first()).toBeVisible();
  expect(errors.filter((e) => /Content.Security.Policy|Refused/i.test(e))).toEqual([]);
});

test("every page has a nonce-based Content Security Policy", async ({ page }) => {
  const response = await page.goto("/");
  const csp = response?.headers()["content-security-policy"] ?? "";

  expect(csp).toMatch(/script-src 'self' 'nonce-[^']+' 'strict-dynamic'/);
  expect(csp).toContain("frame-ancestors 'none'");
});

test("keyboard users can skip to the content", async ({ page }) => {
  await page.goto("/");
  await page.keyboard.press("Tab");
  const skip = page.getByRole("link", { name: "Skip to content" });
  await expect(skip).toBeFocused();
  await expect(skip).toBeVisible();
});

test("unknown pages ask signed-out visitors to sign in, without revealing what exists", async ({
  page,
}) => {
  await page.goto("/does-not-exist");

  await expect(page).toHaveURL(/\/sign-in\?redirect_url=/);
});
