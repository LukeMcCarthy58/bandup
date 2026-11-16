import { expect, test } from "@playwright/test";

import { signedInState } from "../playwright.config";
import { hasTestUser, trackConsoleErrors } from "./helpers";

test.use({ storageState: hasTestUser ? signedInState : { cookies: [], origins: [] } });
test.skip(!hasTestUser, "Set E2E_CLERK_USER_EMAIL to run signed-in tests");

test("signed-in users go from the landing page to their feed", async ({ page }) => {
  await page.goto("/");
  await expect(page).toHaveURL(/\/feed$/);
});

test("app pages load without errors or CSP violations", async ({ page }) => {
  const errors = trackConsoleErrors(page);

  for (const path of ["/feed", "/adverts", "/create", "/messages", "/me"]) {
    await page.goto(path);
    await expect(page.locator("main#main")).toBeVisible();
  }

  expect(errors.filter((e) => /Content.Security.Policy|Refused/i.test(e))).toEqual([]);
});

test("mobile: the bottom tab bar navigates and marks the current tab", async ({
  page,
  isMobile,
}) => {
  test.skip(!isMobile, "Bottom tab bar is mobile-only");

  await page.goto("/feed");
  const tabs = page.getByRole("navigation", { name: "Main" });
  await expect(tabs.getByRole("link", { name: "Feed" })).toHaveAttribute("aria-current", "page");

  await tabs.getByRole("link", { name: "Adverts" }).click();
  await expect(page).toHaveURL(/\/adverts$/);
  await expect(page.getByRole("heading", { level: 1, name: "Adverts" })).toBeVisible();
  await expect(tabs.getByRole("link", { name: "Adverts" })).toHaveAttribute("aria-current", "page");

  await tabs.getByRole("link", { name: "Create" }).click();
  await expect(page).toHaveURL(/\/create$/);
});

test("desktop: the top navigation replaces the bottom tab bar", async ({ page, isMobile }) => {
  test.skip(isMobile, "Top navigation is desktop-only");

  await page.goto("/feed");
  const navs = page.getByRole("navigation", { name: "Main" });
  await expect(navs.filter({ visible: true })).toHaveCount(1);

  await page.getByRole("link", { name: "Messages" }).filter({ visible: true }).click();
  await expect(page.getByRole("heading", { level: 1, name: "Messages" })).toBeVisible();
});

test("the profile page shows who is signed in, with a sign-out button", async ({ page }) => {
  await page.goto("/me");

  await expect(page.getByRole("heading", { level: 1, name: "Your profile" })).toBeVisible();
  await expect(page.getByText(process.env.E2E_CLERK_USER_EMAIL!)).toBeVisible();
  await expect(page.getByRole("button", { name: "Sign out" })).toBeVisible();
});
