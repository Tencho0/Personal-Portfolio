# superior-singularity

A personal technical blog built with [Astro](https://astro.build/), deployed to
Cloudflare Pages.

## Develop

```bash
npm install
npm run dev      # http://localhost:4321
npm run build    # outputs static site to ./dist
npm run preview  # preview the production build locally
```

## Writing posts

Add a Markdown (or `.mdx`) file to `src/content/blog/`. Frontmatter:

```yaml
---
title: 'Post title'
description: 'One-line summary used in lists and meta tags.'
pubDate: 2026-06-21
updatedDate: 2026-06-22   # optional
coverImage: '../../assets/posts/your-image.jpg'   # optional, local + optimized
---
```

Place post images under `src/assets/posts/` and reference them with a relative
path so Astro optimizes them. Code blocks support line highlighting
(`// [!code highlight]`) and diffs (`// [!code ++]` / `// [!code --]`).

## Deploy (Cloudflare Pages — Git integration)

1. Push this repo to GitHub.
2. In the Cloudflare dashboard: **Workers & Pages → Create → Pages → Connect to Git**.
3. Select the repo. Because the app lives in a subdirectory, set:
   - **Root directory:** `v2 - blog/superior-singularity`
   - **Build command:** `npm run build`
   - **Build output directory:** `dist`
   - **Environment variable:** `NODE_VERSION = 22.12.0`
4. Save and deploy. Pushes to `main` deploy to production; pull requests get
   preview URLs.
5. After the first deploy, update `site` in `astro.config.mjs` to the assigned
   `*.pages.dev` URL and commit.
