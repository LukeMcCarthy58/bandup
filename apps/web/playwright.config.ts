import { defineConfig, devices } from "@playwright/test";

// Locally, read Clerk keys and the test user from .env.local (CI sets them as secrets).
try {
  process.loadEnvFile(".env.local");
} catch {
  // No .env.local: fine in CI.
}

const port = 3100;
const isCI = !!process.env.CI;

/** Where the signed-in test user's session is saved by e2e/auth.setup.ts. */
export const signedInState = "playwright/.clerk/user.json";

/**
 * End-to-end tests run the real app in a real browser, at phone and desktop sizes.
 * In CI they run against a production build; locally against the dev server.
 */
export default defineConfig({
  testDir: "./e2e",
  fullyParallel: true,
  forbidOnly: isCI,
  retries: isCI ? 1 : 0,
  reporter: isCI ? [["github"], ["html", { open: "never" }]] : "list",
  use: {
    baseURL: `http://localhost:${port}`,
    trace: "retain-on-failure",
    // Optional: use an already-installed Chrome instead of Playwright's download.
    launchOptions: process.env.PLAYWRIGHT_CHROMIUM_PATH
      ? { executablePath: process.env.PLAYWRIGHT_CHROMIUM_PATH }
      : undefined,
  },
  projects: [
    { name: "setup", testMatch: /auth\.setup\.ts/ },
    {
      name: "mobile",
      use: { ...devices["Pixel 7"] },
      dependencies: ["setup"],
    },
    {
      name: "desktop",
      use: { ...devices["Desktop Chrome"], viewport: { width: 1280, height: 800 } },
      dependencies: ["setup"],
    },
  ],
  webServer: {
    command: isCI ? `pnpm start --port ${port}` : `pnpm dev --port ${port}`,
    url: `http://localhost:${port}`,
    reuseExistingServer: !isCI,
    timeout: 120_000,
  },
});
