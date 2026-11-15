# 0007. Mobile-first design system

- Status: Accepted
- Date: 2026-11-15

## Context
BandUp should feel professional (LinkedIn/Facebook-like) and not look generic or "vibe-coded". Most use will be on phones, and MVP 2 is a native app, so patterns should carry over. Luke wants strong contrast, a primarily white UI with system dark mode, and brand purple `#63099C`.

## Decision
- Build mobile-first (from 360px), with a bottom tab bar on mobile and a top nav on desktop.
- Use a grey canvas with white cards, 8px radius and comfortable density.
- Use Bricolage Grotesque for headings and Inter for body text, with Lucide icons.
- Use purple for accents only. Dark mode uses soft dark grey surfaces and a lighter purple (`#C39BF2`).
- Draw users as circles and bands as squircles.
- Keep motion subtle, honouring reduced motion.
- Use a friendly, plain voice.
- Implement the tokens as CSS variables mapped into Tailwind and shadcn/ui themes. All pairs are verified at WCAG AA or better.
- The full spec is in `docs/design/style-guide.md`.

## Consequences
- One source of truth for tokens; shadcn components inherit the theme.
- The same tokens and navigation pattern port to the Expo app in MVP 2.
- Custom fonts add a small download; mitigated by `next/font` with subsetting and `display: swap`.
- Every UI PR is checked at mobile width and in both themes (PR template).
