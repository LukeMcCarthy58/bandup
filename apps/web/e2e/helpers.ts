import type { Page } from "@playwright/test";

/** Collects console errors, which is how blocked (CSP) resources show up. */
export function trackConsoleErrors(page: Page) {
  const errors: string[] = [];
  page.on("console", (message) => {
    if (message.type() === "error") errors.push(message.text());
  });
  page.on("pageerror", (error) => errors.push(error.message));
  return errors;
}

export const hasTestUser = !!process.env.E2E_CLERK_USER_EMAIL;
