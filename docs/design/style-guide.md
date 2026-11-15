# BandUp style guide

Clean, high-contrast and mobile-first. Professional like LinkedIn, friendly like Facebook. Purple is an accent, not a wallpaper.

## Principles

1. **Readable first.** Every text/background pair meets WCAG AA; most meet AAA.
2. **Mobile-first.** Design at 360px wide, then scale up. Thumb-reachable actions, touch targets of at least 44×44px.
3. **Neutral surfaces, purple accents.** Purple marks what you can act on: primary buttons, links, the active tab, focus rings and badges.
4. **Calm motion.** Quick and subtle, and turned off when the user has reduced motion set.

## Colour tokens

The theme follows the system setting (`prefers-color-scheme`), with no flash of the wrong theme on load.

| Token | Light | Dark | Use |
| --- | --- | --- | --- |
| `--canvas` | `#F3F2F5` | `#0F0F12` | Page background |
| `--card` | `#FFFFFF` | `#18181D` | Cards, sheets, nav bars |
| `--border` | `#E4E2E8` | `#2A2A31` | Card edges, dividers |
| `--border-strong` | `#8C8794` | `#6B6873` | Input outlines (3:1 for UI) |
| `--text` | `#16141A` | `#F4F3F6` | Body and headings |
| `--text-muted` | `#57535E` | `#A9A6B0` | Metadata, timestamps, helper text |
| `--primary` | `#63099C` | `#C39BF2` | Primary buttons, links, active tab, focus ring |
| `--primary-hover` | `#520882` | `#D3B3F7` | Hover/pressed primary |
| `--on-primary` | `#FFFFFF` | `#16141A` | Text on primary buttons |
| `--primary-tint` | `#F1E8F8` | `#2A1F38` | Selected chip / highlight background |
| `--danger` | `#B42318` | `#F97066` | Errors, destructive actions |
| `--success` | `#067647` | `#47CD89` | Success states |

Measured contrast: text on card is 18.3:1 (light) and 16.0:1 (dark). Muted text is 7.5:1 (light) and 7.4:1 (dark). Primary button text is 10.3:1 (light) and 8.1:1 (dark).

Brand purple `#63099C` is too dark to read on a black background, so dark mode uses the lighter `#C39BF2` with dark text on buttons.

## Typography

- **Headings:** Bricolage Grotesque (600–700), loaded with `next/font`
- **Body and UI:** Inter (400/500/600)
- **Base size:** 16px, line height 1.5. Never below 13px (timestamps only).
- **Scale:** 13 · 14 · 16 · 18 · 20 · 24 · 30 px

## Shape and spacing

- **Radius:** 8px on cards, inputs and buttons; full radius only on avatars and badges
- **Spacing:** 4px grid (4, 8, 12, 16, 24, 32)
- **Density:** comfortable, with 16px card padding on mobile and 12px gaps between feed cards
- **Elevation:** borders first; a soft shadow only on overlays (menus, sheets, dialogs)

## Avatars

- **Users:** circle
- **Bands:** squircle (rounded square)

This lets you tell a musician from a band at a glance, which matters because you can post as either. Initials show as a fallback when there's no photo.

## Layout and navigation

- **Mobile:** a top bar (logo, search, notifications) and a bottom tab bar: **Feed · Adverts · ＋ · Messages · Me**. The centre ＋ opens Create (post or advert). Bands live under Me.
- **Tablet and desktop:** a top navigation bar, with the feed in a centre column (max about 600px) and side panels at `lg` and above.
- Respect safe-area insets. Sheets slide up from the bottom on mobile and become dialogs on desktop.

## Icons

Lucide, as outline icons at 1.75px stroke; the active tab uses the filled or bold variant in `--primary`.

## Motion

150–200ms ease-out fades and slides, plus a small scale "pop" on like. Everything is disabled under `prefers-reduced-motion`.

## Voice

Friendly and plain, with a light musical touch. Clarity always wins.

- Do: "No adverts nearby yet. Widen your radius or post your own."
- Do: "Find your band"
- Avoid: "No results found." (cold) and "Nobody's jamming here 🎸" (gimmicky)

Buttons are verbs ("Post", "Request to join", "Send"), and errors say what to do next.

## Accessibility checklist (every UI PR)

- [ ] Works at 360px wide and on desktop
- [ ] Light and dark mode checked
- [ ] Contrast AA or better
- [ ] Keyboard reachable, with a visible focus ring
- [ ] Touch targets at least 44px
- [ ] Images have alt text
- [ ] Motion respects reduced motion
