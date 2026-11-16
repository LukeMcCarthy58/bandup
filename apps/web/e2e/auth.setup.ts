import { clerk, clerkSetup } from "@clerk/testing/playwright";
import { expect, test as setup } from "@playwright/test";

import { signedInState } from "../playwright.config";

setup.describe.configure({ mode: "serial" });

/** Gets a Clerk testing token, so sign-in in tests isn't blocked by bot protection. */
setup("configure Clerk", async () => {
  await clerkSetup();
});

/**
 * Signs in the dedicated test user once and saves the session for the signed-in tests.
 * The user must exist in the Clerk development instance (E2E_CLERK_USER_EMAIL).
 */
setup("sign in the test user", async ({ page }) => {
  const email = process.env.E2E_CLERK_USER_EMAIL;
  setup.skip(!email, "Set E2E_CLERK_USER_EMAIL to run signed-in tests");

  await page.goto("/sign-in");
  await clerk.signIn({ page, emailAddress: email! });
  await page.goto("/feed");
  await expect(page).toHaveURL(/\/feed$/);

  await page.context().storageState({ path: signedInState });
});
