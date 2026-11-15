# web

BandUp's Next.js frontend (App Router, TypeScript, Tailwind CSS v4, shadcn/ui).

```bash
pnpm dev          # http://localhost:3000
pnpm lint
pnpm typecheck
pnpm test         # Vitest + Testing Library
pnpm build
```

- Design tokens live in `src/app/globals.css` and follow `docs/design/style-guide.md`. The theme follows the OS light/dark setting.
- Add shadcn components with `pnpm dlx shadcn@latest add <component>`, then restyle them to the tokens.
- Build mobile-first: start at 360px wide and add `sm:`/`md:` styles upward.
