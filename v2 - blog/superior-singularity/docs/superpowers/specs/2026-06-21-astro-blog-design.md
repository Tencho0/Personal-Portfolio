# Personal Technical Blog — Design Spec

**Date:** 2026-06-21
**Status:** Approved (design); pending implementation plan
**Project:** `superior-singularity` (Astro blog, in `Personal-Portfolio` repo)

## Summary

A pure, static, **technical/developer** blog built by refactoring the existing
official Astro blog starter that is already scaffolded in this directory. The
goal is a **minimal, typographic** site that does not read as the default
template, with dark mode, polished syntax-highlighted code blocks, and locally
optimized images. Deployed to **Cloudflare Pages** via GitHub Git integration on
a free `*.pages.dev` domain for now.

Out of scope for v1: portfolio sections, tags/categories, and search. The design
keeps content in typed content collections so tags/search can be added later
without restructuring.

## Decisions (from brainstorming)

| Question | Decision |
|---|---|
| Scope | Pure blog (no portfolio) |
| Content / audience | Technical / dev posts, developer audience |
| Aesthetic | Minimal / typographic |
| Features (v1) | Dark-mode toggle, syntax-highlighted code, optimized images |
| Images | Local files, Astro-optimized (resize + webp/avif) |
| Deploy | Cloudflare Pages, GitHub Git integration (auto-deploy on push) |
| Domain | `*.pages.dev` placeholder for now |
| Build approach | A — refactor the existing starter in place |

## Build approach

Refactor the official Astro blog starter already present (`src/`), keeping its
proven plumbing — content collections, RSS, sitemap, pagination, layouts — and
restyling/extending it. We do **not** rebuild from scratch or adopt a
third-party theme.

## Section 1 — Design system

Everything drives off CSS custom-property **tokens** in one place
(`src/styles/global.css`) so the palette and type scale are changeable centrally.

**Typography** (replace the starter's Atkinson font; all self-hosted via Astro's
font system — no Google Fonts requests at runtime):
- **Body:** Source Serif 4 (variable). ~18–19px, line-height ~1.65, measure ~68ch.
- **Headings:** Space Grotesk, set tight.
- **Code:** JetBrains Mono.

**Color:**
- Built on CSS custom properties: a near-black / near-white text+background pair
  (not pure `#000`/`#fff`), one **warm clay/rust accent** (paired light/dark
  values, single token, easily swapped), plus a few neutral grays.
- Light + dark themes = two token sets.

**Dark mode:**
- Respects `prefers-color-scheme` on first visit; user can override; choice
  persists in `localStorage`.
- Theme applied **before paint** via an inline head script to avoid flash (FOUC).

**Spacing & layout:**
- Single centered readable column, no sidebars. Consistent vertical rhythm from a
  small spacing scale. Whitespace carries the design.

## Section 2 — Structure & components

**Pages** (restyle existing where present):
- **Home** (`/`) — short intro line + reverse-chronological list of recent posts
  (title, date, one-line description). No hero fluff.
- **Post** (`/blog/[slug]`) — reading view: title, date, optional cover image,
  content. Primary home of typography + code-block styling.
- **Blog index** (`/blog`) — full archive list of all posts.
- **About** (`/about`) — bio.
- **RSS** (`/rss.xml`) + **sitemap** — keep existing wiring.
- **404** — simple styled not-found page (new).

**Components** (refactored from starter set):
- `BaseHead` — meta, SEO/OpenGraph, font preloads, no-flash theme script.
- `Header` — site title, nav (Blog, About), dark-mode toggle.
- `Footer` — minimal: copyright, RSS link, optional social.
- `ThemeToggle` — dark/light switch (new).
- `FormattedDate`, post list item — small presentational pieces.

**Content model:**
- Astro **content collections** at `src/content/blog/`, one Markdown/MDX file per
  post.
- Typed frontmatter schema: `title`, `description`, `pubDate`, optional
  `updatedDate`, optional `coverImage`. Malformed posts fail at build.

## Section 3 — Content, code & images

**Authoring:**
- Markdown by default; MDX when a component is needed inline.
- Delete starter placeholder posts; add one real intro post so the site isn't
  empty.

**Code blocks** (priority for a dev blog):
- Astro's built-in **Shiki**, tuned with a dual light/dark theme that switches via
  CSS variables (no JS reflow).
- **Copy button** per block; line highlighting / line numbers via Shiki
  transformers.
- Inline `code` styled to match.

**Images:**
- Local + **optimized**: co-locate with the post, reference in Markdown; Astro's
  asset pipeline resizes and serves webp/avif with explicit dimensions (no layout
  shift).
- Optional **cover image** in frontmatter → rendered at top of post and used in
  OpenGraph/social cards.
- Default content width for images, with an opt-in wider "bleed" option.

## Section 4 — Deploy

- **Static** build, no SSR adapter.
- **Cloudflare Pages** via GitHub Git integration: build command `npm run build`,
  output dir `dist`, Node 22.
- Push to `main` → production deploy; PRs → preview URLs.
- `site` URL set to a `*.pages.dev` placeholder in `astro.config.mjs` and
  `src/consts.ts`; swappable when a custom domain is added.
- Wrangler not required.

## Success criteria

1. `npm run build` produces a static `dist/` with no errors.
2. Site looks deliberate and minimal — not the default starter — in light and dark
   modes, with no theme flash on load.
3. A real post renders with styled, copyable, dual-theme code blocks and an
   optimized local image + cover image.
4. RSS and sitemap generate correctly; OpenGraph tags present.
5. Lighthouse: performance and accessibility in good shape (single column,
   self-hosted fonts, optimized images).
6. Repo builds cleanly on Cloudflare Pages with the documented settings.

## Deferred (not v1)

- Portfolio sections
- Tags / categories browsing
- Client-side search (e.g. Pagefind)
- Comments, analytics, custom domain
