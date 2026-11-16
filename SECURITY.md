# Security policy

## Reporting a vulnerability

Please **don't** open a public issue for security problems.

Report it privately through GitHub instead: go to the repo's **Security** tab and choose **Report a vulnerability**. Include:

- what you found and where (URL, endpoint or file),
- steps to reproduce,
- the impact you think it has.

You'll get an acknowledgement within 5 working days. Please give us reasonable time to fix the issue before sharing it publicly.

## Supported versions

Only the latest version on `main` (and the live deployment, once launched) is supported.

## How BandUp stays secure

- **Dependencies:** Dependabot raises weekly update PRs and immediate security fixes. The .NET build fails on known-vulnerable packages (NuGet audit).
- **Code scanning:** CodeQL analyses C#, TypeScript and GitHub Actions workflows on every PR and weekly.
- **Secrets:** secret scanning and push protection are on. Local secrets live in .NET user-secrets and untracked `.env` files, never in the repo.
- **CI:** workflows use least-privilege `permissions:`, and third-party actions are pinned to commit SHAs.
- **Application:** see the security model in [`docs/architecture.md`](docs/architecture.md).
