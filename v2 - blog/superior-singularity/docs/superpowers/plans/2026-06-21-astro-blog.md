# Personal Technical Blog Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Refactor the scaffolded Astro blog starter into a minimal, typographic, dark-mode-capable technical blog deployable to Cloudflare Pages.

**Architecture:** Keep the starter's proven plumbing (content collections, RSS, sitemap, layouts) and restyle/extend it. All visual decisions flow from CSS custom-property tokens in one file. Fonts are self-hosted via Astro's Fonts API (Google provider, downloaded and served locally at build). Code blocks use Astro's built-in Shiki with dual light/dark themes plus a client-side copy button. Images are local and optimized through Astro's asset pipeline. Output is fully static — no SSR adapter.

**Tech Stack:** Astro 6, MDX, `@astrojs/rss`, `@astrojs/sitemap`, Shiki (built-in) + `@shikijs/transformers`, `sharp` (already present), Cloudflare Pages.

## Global Constraints

- **Node:** `>=22.12.0` (already in `package.json` `engines`).
- **Static only:** no SSR adapter; `astro build` must emit `dist/`.
- **Fonts:** self-hosted only — no runtime requests to Google Fonts. Use `fontProviders.google()` (downloads at build) via Astro's Fonts API.
- **Tokens:** all color/type/spacing values live as CSS custom properties in `src/styles/global.css`. No hard-coded colors in component `<style>` blocks — reference tokens.
- **Theme tokens (exact values, use verbatim):**
  - Light: `--color-bg: #faf9f6; --color-surface: #f1efe9; --color-text: #1b1a17; --color-text-muted: #5c5950; --color-border: #e2ded3; --color-accent: #b5512f; --color-accent-hover: #993f22;`
  - Dark: `--color-bg: #16150f; --color-surface: #211f17; --color-text: #ece8dd; --color-text-muted: #a8a395; --color-border: #322f25; --color-accent: #e07a52; --color-accent-hover: #ef9067;`
- **Font CSS variables (exact names):** `--font-display` (Space Grotesk), `--font-body` (Source Serif 4), `--font-mono` (JetBrains Mono).
- **Frontmatter schema field for the post image is named `coverImage`** (rename from the starter's `heroImage`). All references must use `coverImage`.
- **No `prefers-color-scheme` flash:** theme class must be applied to `<html>` by an inline script before first paint.
- **No lorem ipsum** in shipped content — use real (if placeholder-quality) copy the user can edit.

---

## File Structure

**Modify:**
- `astro.config.mjs` — site URL, fonts (3 families via Google provider), `markdown.shikiConfig` dual themes + transformers.
- `package.json` — add `@shikijs/transformers` dependency.
- `src/consts.ts` — real site title/description + author/social constants.
- `src/content.config.ts` — rename `heroImage` → `coverImage`.
- `src/styles/global.css` — full token-based rewrite (typography, color, dark mode, spacing, base elements, code-block dual-theme CSS).
- `src/components/BaseHead.astro` — font preloads for 3 families, OG fallback image, no Atkinson.
- `src/components/Header.astro` — nav (Home/Blog/About) + theme toggle, token-based styles, remove Astro social links.
- `src/components/Footer.astro` — minimal footer: copyright + RSS link + optional social, token-based.
- `src/layouts/BlogPost.astro` — single-column prose layout, `coverImage`, token styles.
- `src/pages/index.astro` — intro + recent-posts list.
- `src/pages/blog/index.astro` — full archive list.
- `src/pages/about.astro` — real bio, `coverImage` prop.

**Create:**
- `src/components/ThemeToggle.astro` — the dark/light switch button + its client script.
- `src/components/PostListItem.astro` — one row in a post list (reused by home + archive).
- `src/scripts/copy-code.ts` — client script that adds copy buttons to code blocks.
- `src/pages/404.astro` — styled not-found page.
- `src/content/blog/hello-world.md` — one real intro post demonstrating code blocks + a local cover image.
- `src/assets/posts/hello-world-cover.jpg` — a local cover image for the intro post (copy an existing placeholder for now).

**Delete:**
- `src/content/blog/first-post.md`, `second-post.md`, `third-post.md`, `markdown-style-guide.md`, `using-mdx.mdx` — starter placeholder posts.
- `src/assets/fonts/atkinson-regular.woff`, `atkinson-bold.woff` — unused after font swap.

---

## Methodology note

No JS test framework is installed and this is a presentational static site, so the per-task "test" cycle is:
1. `npm run build` completes with **exit 0** (catches schema, import, and type errors).
2. Where behavior is visual or interactive, a `npm run dev` check at the noted URL confirms the intended result.

Treat a non-zero build exit as a failing test: fix before committing.

---

### Task 1: Config, dependencies, and constants

**Files:**
- Modify: `astro.config.mjs`
- Modify: `package.json` (via npm install)
- Modify: `src/consts.ts`

**Interfaces:**
- Produces: CSS variables `--font-display`, `--font-body`, `--font-mono` available to all pages once `<Font>` tags are emitted (Task 4). Exports `SITE_TITLE`, `SITE_DESCRIPTION`, `AUTHOR_NAME`, `GITHUB_URL` from `consts.ts`.

- [ ] **Step 1: Install the Shiki transformers package**

Run:
```bash
npm install @shikijs/transformers
```
Expected: `package.json` `dependencies` now lists `@shikijs/transformers`; no errors.

- [ ] **Step 2: Replace `astro.config.mjs` with the full configuration**

Write `astro.config.mjs`:
```js
// @ts-check

import mdx from '@astrojs/mdx';
import sitemap from '@astrojs/sitemap';
import { defineConfig, fontProviders } from 'astro/config';
import {
	transformerNotationHighlight,
	transformerNotationDiff,
} from '@shikijs/transformers';

// https://astro.build/config
export default defineConfig({
	// TODO: replace with the real *.pages.dev URL once the Pages project exists.
	site: 'https://superior-singularity.pages.dev',
	integrations: [mdx(), sitemap()],
	markdown: {
		shikiConfig: {
			themes: {
				light: 'github-light',
				dark: 'github-dark',
			},
			// Required so dual-theme CSS variables are emitted instead of inline colors.
			defaultColor: false,
			wrap: false,
			transformers: [
				transformerNotationHighlight(),
				transformerNotationDiff(),
			],
		},
	},
	fonts: [
		{
			provider: fontProviders.google(),
			name: 'Space Grotesk',
			cssVariable: '--font-display',
			weights: [500, 700],
			fallbacks: ['system-ui', 'sans-serif'],
		},
		{
			provider: fontProviders.google(),
			name: 'Source Serif 4',
			cssVariable: '--font-body',
			weights: [400, 600, 700],
			styles: ['normal', 'italic'],
			fallbacks: ['Georgia', 'serif'],
		},
		{
			provider: fontProviders.google(),
			name: 'JetBrains Mono',
			cssVariable: '--font-mono',
			weights: [400, 700],
			fallbacks: ['ui-monospace', 'monospace'],
		},
	],
});
```

- [ ] **Step 3: Replace `src/consts.ts`**

Write `src/consts.ts`:
```ts
// Place any global data in this file.
// You can import this data from anywhere in your site by using the `import` keyword.

export const SITE_TITLE = 'Tencho Bostandzhiev';
export const SITE_DESCRIPTION = 'Notes on software engineering, tools, and things I build.';
export const AUTHOR_NAME = 'Tencho Bostandzhiev';
export const GITHUB_URL = 'https://github.com/';
```

- [ ] **Step 4: Build to verify config is valid**

Run:
```bash
npm run build
```
Expected: exit 0. Astro downloads the three font families at build (first run may take longer). A pre-existing build may still reference the old `heroImage`/placeholder posts — that is fine; those are removed in later tasks. If the build fails for a reason **other** than missing later-task changes (e.g., a config syntax error), fix it now.

- [ ] **Step 5: Commit**

```bash
git add astro.config.mjs package.json package-lock.json src/consts.ts
git commit -m "chore: configure fonts, shiki dual themes, and site constants"
```

---

### Task 2: Design-system tokens and global stylesheet

**Files:**
- Modify: `src/styles/global.css`

**Interfaces:**
- Produces: token CSS variables (colors above, `--font-*`, spacing scale `--space-1..6`, `--measure`), base element styling, `.prose` rules, and `.astro-code` dual-theme rules consumed by every component/layout. Theme is switched by a `dark` class on `<html>` (added in Task 3).

- [ ] **Step 1: Replace `src/styles/global.css` entirely**

Write `src/styles/global.css`:
```css
/* Design tokens — single source of truth for color, type, and spacing. */
:root {
	/* Light theme (default) */
	--color-bg: #faf9f6;
	--color-surface: #f1efe9;
	--color-text: #1b1a17;
	--color-text-muted: #5c5950;
	--color-border: #e2ded3;
	--color-accent: #b5512f;
	--color-accent-hover: #993f22;

	/* Typography */
	--font-body: 'Source Serif 4', Georgia, serif;
	--font-display: 'Space Grotesk', system-ui, sans-serif;
	--font-mono: 'JetBrains Mono', ui-monospace, monospace;

	/* Spacing scale */
	--space-1: 0.25rem;
	--space-2: 0.5rem;
	--space-3: 1rem;
	--space-4: 1.5rem;
	--space-5: 2.5rem;
	--space-6: 4rem;

	/* Reading measure */
	--measure: 68ch;

	color-scheme: light;
}

html.dark {
	--color-bg: #16150f;
	--color-surface: #211f17;
	--color-text: #ece8dd;
	--color-text-muted: #a8a395;
	--color-border: #322f25;
	--color-accent: #e07a52;
	--color-accent-hover: #ef9067;

	color-scheme: dark;
}

* {
	box-sizing: border-box;
}

html {
	background: var(--color-bg);
}

body {
	margin: 0;
	padding: 0;
	font-family: var(--font-body);
	font-size: 19px;
	line-height: 1.65;
	color: var(--color-text);
	background: var(--color-bg);
	text-align: left;
	word-wrap: break-word;
	overflow-wrap: break-word;
	-webkit-font-smoothing: antialiased;
	transition: background-color 0.2s ease, color 0.2s ease;
}

@media (max-width: 720px) {
	body {
		font-size: 17px;
	}
}

main {
	width: var(--measure);
	max-width: calc(100% - 2 * var(--space-3));
	margin: 0 auto;
	padding: var(--space-6) 0;
}

h1, h2, h3, h4, h5, h6 {
	margin: 0 0 var(--space-3) 0;
	font-family: var(--font-display);
	font-weight: 700;
	line-height: 1.15;
	letter-spacing: -0.02em;
	color: var(--color-text);
}

h1 { font-size: 2.6rem; }
h2 { font-size: 1.9rem; margin-top: var(--space-5); }
h3 { font-size: 1.45rem; margin-top: var(--space-4); }
h4 { font-size: 1.2rem; }

@media (max-width: 720px) {
	h1 { font-size: 2rem; }
	h2 { font-size: 1.6rem; }
}

p { margin: 0 0 var(--space-3) 0; }

a {
	color: var(--color-accent);
	text-decoration: underline;
	text-underline-offset: 2px;
	text-decoration-thickness: 1px;
}
a:hover { color: var(--color-accent-hover); }

strong, b { font-weight: 700; }

ul, ol { padding-left: var(--space-4); }
li { margin-bottom: var(--space-2); }

img {
	max-width: 100%;
	height: auto;
	border-radius: 6px;
}

hr {
	border: none;
	border-top: 1px solid var(--color-border);
	margin: var(--space-5) 0;
}

blockquote {
	border-left: 3px solid var(--color-accent);
	padding-left: var(--space-3);
	margin: var(--space-4) 0;
	color: var(--color-text-muted);
	font-style: italic;
}

table { width: 100%; border-collapse: collapse; }
th, td {
	border: 1px solid var(--color-border);
	padding: var(--space-2) var(--space-3);
	text-align: left;
}

/* Inline code */
:not(pre) > code {
	font-family: var(--font-mono);
	font-size: 0.85em;
	padding: 0.15em 0.4em;
	background: var(--color-surface);
	border: 1px solid var(--color-border);
	border-radius: 4px;
}

/* Shiki code blocks — dual theme.
   With defaultColor:false, Shiki emits --shiki-light / --shiki-dark CSS vars.
   We bind the light values by default and swap to dark under html.dark. */
.astro-code {
	font-family: var(--font-mono);
	font-size: 0.9rem;
	line-height: 1.6;
	padding: var(--space-3);
	border-radius: 8px;
	border: 1px solid var(--color-border);
	overflow-x: auto;
	background-color: var(--shiki-light-bg);
}
.astro-code,
.astro-code span {
	color: var(--shiki-light);
}
html.dark .astro-code {
	background-color: var(--shiki-dark-bg);
}
html.dark .astro-code,
html.dark .astro-code span {
	color: var(--shiki-dark);
}

/* Line highlighting (transformerNotationHighlight) */
.astro-code .highlighted {
	display: inline-block;
	width: 100%;
	background: rgba(120, 120, 120, 0.18);
	margin: 0 calc(-1 * var(--space-3));
	padding: 0 var(--space-3);
}
/* Diff notation (transformerNotationDiff) */
.astro-code .diff.add { background: rgba(80, 200, 120, 0.18); }
.astro-code .diff.remove { background: rgba(220, 80, 80, 0.18); }

/* Code block wrapper + copy button (see src/scripts/copy-code.ts) */
.code-block {
	position: relative;
	margin: var(--space-4) 0;
}
.code-block .copy-btn {
	position: absolute;
	top: var(--space-2);
	right: var(--space-2);
	font-family: var(--font-display);
	font-size: 0.7rem;
	letter-spacing: 0.05em;
	text-transform: uppercase;
	padding: 0.25em 0.6em;
	color: var(--color-text-muted);
	background: var(--color-bg);
	border: 1px solid var(--color-border);
	border-radius: 4px;
	cursor: pointer;
	opacity: 0;
	transition: opacity 0.15s ease;
}
.code-block:hover .copy-btn { opacity: 1; }
.code-block .copy-btn:hover { color: var(--color-accent); }

/* Prose reading column tweaks */
.prose p { margin-bottom: var(--space-4); }
.prose img { margin: var(--space-4) auto; display: block; }

/* Visually-hidden utility */
.sr-only {
	border: 0;
	padding: 0;
	margin: 0;
	position: absolute !important;
	height: 1px;
	width: 1px;
	overflow: hidden;
	clip: rect(1px, 1px, 1px, 1px);
	clip-path: inset(50%);
	white-space: nowrap;
}
```

- [ ] **Step 2: Build to verify the stylesheet parses and pages still compile**

Run:
```bash
npm run build
```
Expected: exit 0 (later-task content issues aside). CSS is not type-checked, but Astro will fail if `global.css` can't be imported.

- [ ] **Step 3: Commit**

```bash
git add src/styles/global.css
git commit -m "feat: token-based design system with light/dark themes"
```

---

### Task 3: No-flash theme script and ThemeToggle component

**Files:**
- Create: `src/components/ThemeToggle.astro`
- Modify: `src/components/BaseHead.astro` (add the pre-paint inline script only; font/OG changes are Task 4)

**Interfaces:**
- Consumes: `html.dark` styling from Task 2.
- Produces: `<ThemeToggle />` component (no props) used by `Header.astro` (Task 5). A `theme` key in `localStorage` with values `'light'` | `'dark'`. Global function not exported — toggle is self-contained.

- [ ] **Step 1: Add the pre-paint theme script to `BaseHead.astro`**

In `src/components/BaseHead.astro`, add this block as the **first** element of the rendered output, immediately after the `---` frontmatter close and before `<meta charset…>`:
```astro
<!-- Apply theme before paint to avoid FOUC -->
<script is:inline>
	(() => {
		const stored = localStorage.getItem('theme');
		const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
		const isDark = stored ? stored === 'dark' : prefersDark;
		document.documentElement.classList.toggle('dark', isDark);
	})();
</script>
```

- [ ] **Step 2: Create `src/components/ThemeToggle.astro`**

Write `src/components/ThemeToggle.astro`:
```astro
---
---
<button id="theme-toggle" type="button" aria-label="Toggle color theme">
	<svg class="icon-sun" width="20" height="20" viewBox="0 0 24 24" fill="none"
		stroke="currentColor" stroke-width="2" stroke-linecap="round" aria-hidden="true">
		<circle cx="12" cy="12" r="4"></circle>
		<path d="M12 2v2M12 20v2M2 12h2M20 12h2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M19.1 4.9l-1.4 1.4M6.3 17.7l-1.4 1.4"></path>
	</svg>
	<svg class="icon-moon" width="20" height="20" viewBox="0 0 24 24" fill="none"
		stroke="currentColor" stroke-width="2" stroke-linecap="round" aria-hidden="true">
		<path d="M21 12.8A9 9 0 1 1 11.2 3a7 7 0 0 0 9.8 9.8z"></path>
	</svg>
</button>

<style>
	#theme-toggle {
		display: inline-flex;
		align-items: center;
		justify-content: center;
		width: 2.25rem;
		height: 2.25rem;
		padding: 0;
		color: var(--color-text-muted);
		background: transparent;
		border: 1px solid var(--color-border);
		border-radius: 6px;
		cursor: pointer;
		transition: color 0.15s ease, border-color 0.15s ease;
	}
	#theme-toggle:hover {
		color: var(--color-accent);
		border-color: var(--color-accent);
	}
	.icon-moon { display: none; }
	:global(html.dark) #theme-toggle .icon-sun { display: none; }
	:global(html.dark) #theme-toggle .icon-moon { display: inline; }
</style>

<script>
	const btn = document.getElementById('theme-toggle');
	btn?.addEventListener('click', () => {
		const isDark = document.documentElement.classList.toggle('dark');
		localStorage.setItem('theme', isDark ? 'dark' : 'light');
	});
</script>
```

- [ ] **Step 2b: Build to verify both files compile**

Run:
```bash
npm run build
```
Expected: exit 0 (modulo later-task content). The toggle is not yet placed in the header — that happens in Task 5.

- [ ] **Step 3: Commit**

```bash
git add src/components/ThemeToggle.astro src/components/BaseHead.astro
git commit -m "feat: pre-paint theme script and theme toggle component"
```

---

### Task 4: BaseHead — fonts and Open Graph

**Files:**
- Modify: `src/components/BaseHead.astro`

**Interfaces:**
- Consumes: `--font-*` cssVariables defined in `astro.config.mjs` (Task 1); `SITE_TITLE` from `consts.ts`.
- Produces: `<Font>` preload tags so the three families load; OG/Twitter meta with a working fallback image.

- [ ] **Step 1: Update the font emission and fallback image in `BaseHead.astro`**

In `src/components/BaseHead.astro`:

1. Change the fallback image import from the blog placeholder to a stable asset that still exists. Replace:
```astro
import FallbackImage from '../assets/blog-placeholder-1.jpg';
```
with:
```astro
import FallbackImage from '../assets/blog-placeholder-1.jpg';
// NOTE: blog-placeholder-1.jpg is retained as the OG fallback image.
```
(Keep `blog-placeholder-1.jpg` on disk — do not delete it.)

2. Replace the single Atkinson font line:
```astro
<Font cssVariable="--font-atkinson" preload />
```
with all three families:
```astro
<Font cssVariable="--font-body" preload />
<Font cssVariable="--font-display" preload />
<Font cssVariable="--font-mono" />
```

- [ ] **Step 2: Build to verify fonts emit and head compiles**

Run:
```bash
npm run build
```
Expected: exit 0 (modulo later content tasks). Built HTML `<head>` contains `@font-face` style/preload output for the three families and no reference to `--font-atkinson`.

- [ ] **Step 3: Commit**

```bash
git add src/components/BaseHead.astro
git commit -m "feat: preload Source Serif 4, Space Grotesk, JetBrains Mono in head"
```

---

### Task 5: Header and Footer

**Files:**
- Modify: `src/components/Header.astro`
- Modify: `src/components/Footer.astro`

**Interfaces:**
- Consumes: `SITE_TITLE`, `AUTHOR_NAME`, `GITHUB_URL` from `consts.ts`; `<ThemeToggle />` from Task 3; `HeaderLink` (existing).
- Produces: site chrome used by every page.

- [ ] **Step 1: Replace `src/components/Header.astro`**

Write `src/components/Header.astro`:
```astro
---
import { SITE_TITLE } from '../consts';
import HeaderLink from './HeaderLink.astro';
import ThemeToggle from './ThemeToggle.astro';
---

<header>
	<nav>
		<a class="site-title" href="/">{SITE_TITLE}</a>
		<div class="nav-right">
			<div class="links">
				<HeaderLink href="/">Home</HeaderLink>
				<HeaderLink href="/blog">Blog</HeaderLink>
				<HeaderLink href="/about">About</HeaderLink>
			</div>
			<ThemeToggle />
		</div>
	</nav>
</header>

<style>
	header {
		border-bottom: 1px solid var(--color-border);
	}
	nav {
		width: var(--measure);
		max-width: calc(100% - 2 * var(--space-3));
		margin: 0 auto;
		padding: var(--space-3) 0;
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: var(--space-3);
	}
	.site-title {
		font-family: var(--font-display);
		font-weight: 700;
		font-size: 1.05rem;
		color: var(--color-text);
		text-decoration: none;
		letter-spacing: -0.01em;
	}
	.nav-right {
		display: flex;
		align-items: center;
		gap: var(--space-4);
	}
	.links {
		display: flex;
		gap: var(--space-3);
		font-family: var(--font-display);
		font-size: 0.95rem;
	}
	@media (max-width: 520px) {
		.links { gap: var(--space-2); font-size: 0.85rem; }
		.nav-right { gap: var(--space-3); }
	}
</style>
```

- [ ] **Step 2: Update `HeaderLink.astro` active style to use tokens**

In `src/components/HeaderLink.astro`, replace the `<style>` block with:
```astro
<style>
	a {
		display: inline-block;
		text-decoration: none;
		color: var(--color-text-muted);
	}
	a:hover { color: var(--color-text); }
	a.active {
		color: var(--color-accent);
		font-weight: 600;
	}
</style>
```

- [ ] **Step 3: Replace `src/components/Footer.astro`**

Write `src/components/Footer.astro`:
```astro
---
import { AUTHOR_NAME, GITHUB_URL } from '../consts';
const year = new Date().getFullYear();
---

<footer>
	<div class="inner">
		<span>&copy; {year} {AUTHOR_NAME}</span>
		<div class="links">
			<a href="/rss.xml">RSS</a>
			<a href={GITHUB_URL} target="_blank" rel="noopener">GitHub</a>
		</div>
	</div>
</footer>

<style>
	footer {
		border-top: 1px solid var(--color-border);
		color: var(--color-text-muted);
		font-family: var(--font-display);
		font-size: 0.85rem;
	}
	.inner {
		width: var(--measure);
		max-width: calc(100% - 2 * var(--space-3));
		margin: 0 auto;
		padding: var(--space-4) 0;
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: var(--space-3);
	}
	.links { display: flex; gap: var(--space-3); }
	.links a { color: var(--color-text-muted); text-decoration: none; }
	.links a:hover { color: var(--color-accent); }
</style>
```

- [ ] **Step 4: Build, then dev-check the toggle and nav**

Run:
```bash
npm run build
```
Expected: exit 0 (modulo content tasks). Then run `npm run dev` and open `http://localhost:4321/`:
- Header shows title + Home/Blog/About + toggle.
- Clicking the toggle flips light/dark instantly and the icon swaps; reloading preserves the choice; no flash on reload.

- [ ] **Step 5: Commit**

```bash
git add src/components/Header.astro src/components/HeaderLink.astro src/components/Footer.astro
git commit -m "feat: restyle header (with theme toggle) and footer"
```

---

### Task 6: Content schema rename and copy-code script

**Files:**
- Modify: `src/content.config.ts`
- Create: `src/scripts/copy-code.ts`
- Modify: `src/components/BaseHead.astro` (import the copy-code script once, globally)

**Interfaces:**
- Produces: frontmatter field `coverImage` (replaces `heroImage`); a global client script that wraps each `pre.astro-code` in `.code-block` and injects a copy button styled in Task 2.

- [ ] **Step 1: Rename `heroImage` → `coverImage` in the schema**

In `src/content.config.ts`, change:
```ts
heroImage: z.optional(image()),
```
to:
```ts
coverImage: z.optional(image()),
```

- [ ] **Step 2: Create `src/scripts/copy-code.ts`**

Write `src/scripts/copy-code.ts`:
```ts
// Wraps each Shiki code block in a positioned container and adds a copy button.
function enhanceCodeBlocks() {
	const blocks = document.querySelectorAll('pre.astro-code');
	blocks.forEach((pre) => {
		if (pre.parentElement?.classList.contains('code-block')) return;
		const wrapper = document.createElement('div');
		wrapper.className = 'code-block';
		pre.parentNode?.insertBefore(wrapper, pre);
		wrapper.appendChild(pre);

		const btn = document.createElement('button');
		btn.className = 'copy-btn';
		btn.type = 'button';
		btn.textContent = 'Copy';
		btn.addEventListener('click', async () => {
			const code = pre.querySelector('code')?.textContent ?? '';
			try {
				await navigator.clipboard.writeText(code);
				btn.textContent = 'Copied';
				setTimeout(() => (btn.textContent = 'Copy'), 1500);
			} catch {
				btn.textContent = 'Error';
				setTimeout(() => (btn.textContent = 'Copy'), 1500);
			}
		});
		wrapper.appendChild(btn);
	});
}

enhanceCodeBlocks();
```

- [ ] **Step 3: Load the copy-code script globally from `BaseHead.astro`**

In `src/components/BaseHead.astro`, add this near the end of the rendered output (after the meta tags):
```astro
<script>
	import '../scripts/copy-code.ts';
</script>
```

- [ ] **Step 4: Build**

Run:
```bash
npm run build
```
Expected: build will now FAIL referencing `heroImage` in `BlogPost.astro`, `about.astro`, and `blog/index.astro` — those are fixed in Tasks 7–9. This confirms the schema rename took effect. If it fails for any **other** reason (e.g., a TypeScript error in `copy-code.ts`), fix that now.

- [ ] **Step 5: Commit**

```bash
git add src/content.config.ts src/scripts/copy-code.ts src/components/BaseHead.astro
git commit -m "feat: rename coverImage field and add code copy-button script"
```

---

### Task 7: BlogPost layout

**Files:**
- Modify: `src/layouts/BlogPost.astro`

**Interfaces:**
- Consumes: `coverImage` (Task 6), `BaseHead`, `Header`, `Footer`, `FormattedDate`.
- Produces: the reading view for every post and the About page.

- [ ] **Step 1: Replace `src/layouts/BlogPost.astro`**

Write `src/layouts/BlogPost.astro`:
```astro
---
import { Image } from 'astro:assets';
import type { CollectionEntry } from 'astro:content';
import BaseHead from '../components/BaseHead.astro';
import Footer from '../components/Footer.astro';
import FormattedDate from '../components/FormattedDate.astro';
import Header from '../components/Header.astro';

type Props = CollectionEntry<'blog'>['data'];

const { title, description, pubDate, updatedDate, coverImage } = Astro.props;
---

<html lang="en">
	<head>
		<BaseHead title={title} description={description} image={coverImage} />
	</head>
	<body>
		<Header />
		<main>
			<article class="prose">
				<header class="post-header">
					<h1>{title}</h1>
					<p class="post-meta">
						<FormattedDate date={pubDate} />
						{updatedDate && (
							<span class="updated"> · updated <FormattedDate date={updatedDate} /></span>
						)}
					</p>
				</header>
				{coverImage && (
					<Image class="cover" src={coverImage} alt="" width={1200} height={600} />
				)}
				<slot />
			</article>
		</main>
		<Footer />
	</body>
</html>

<style>
	.post-header { margin-bottom: var(--space-4); }
	.post-header h1 { margin-bottom: var(--space-2); }
	.post-meta {
		font-family: var(--font-display);
		font-size: 0.9rem;
		color: var(--color-text-muted);
		margin: 0;
	}
	.cover {
		width: 100%;
		height: auto;
		border-radius: 8px;
		margin-bottom: var(--space-5);
	}
</style>
```

Note: `BaseHead`'s `image` prop type is `ImageMetadata`; `coverImage` is `ImageMetadata | undefined`, and `BaseHead` already defaults `image` when undefined — passing `image={coverImage}` is valid.

- [ ] **Step 2: Build**

Run:
```bash
npm run build
```
Expected: still FAILS on `about.astro` and `blog/index.astro` `heroImage` references (Tasks 8–9), but `BlogPost.astro` itself must no longer be the cause. If the error names `BlogPost.astro`, fix before continuing.

- [ ] **Step 3: Commit**

```bash
git add src/layouts/BlogPost.astro
git commit -m "feat: single-column prose post layout with cover image"
```

---

### Task 8: Post list item, home page, and blog archive

**Files:**
- Create: `src/components/PostListItem.astro`
- Modify: `src/pages/index.astro`
- Modify: `src/pages/blog/index.astro`

**Interfaces:**
- Consumes: `FormattedDate`; `getCollection('blog')`; `coverImage` field (unused in list, kept in schema).
- Produces: `<PostListItem post={...} />` where `post` is a `CollectionEntry<'blog'>`.

- [ ] **Step 1: Create `src/components/PostListItem.astro`**

Write `src/components/PostListItem.astro`:
```astro
---
import type { CollectionEntry } from 'astro:content';
import FormattedDate from './FormattedDate.astro';

interface Props {
	post: CollectionEntry<'blog'>;
}
const { post } = Astro.props;
---

<li class="post-item">
	<a href={`/blog/${post.id}/`}>
		<span class="title">{post.data.title}</span>
		<FormattedDate date={post.data.pubDate} />
	</a>
	{post.data.description && <p class="desc">{post.data.description}</p>}
</li>

<style>
	.post-item {
		list-style: none;
		padding: var(--space-3) 0;
		border-bottom: 1px solid var(--color-border);
	}
	.post-item a {
		display: flex;
		align-items: baseline;
		justify-content: space-between;
		gap: var(--space-3);
		text-decoration: none;
		color: var(--color-text);
	}
	.post-item .title {
		font-family: var(--font-display);
		font-weight: 500;
		font-size: 1.1rem;
	}
	.post-item a:hover .title { color: var(--color-accent); }
	.post-item :global(time) {
		flex-shrink: 0;
		font-family: var(--font-display);
		font-size: 0.8rem;
		color: var(--color-text-muted);
	}
	.desc {
		margin: var(--space-1) 0 0 0;
		color: var(--color-text-muted);
		font-size: 0.95rem;
	}
</style>
```

- [ ] **Step 2: Replace `src/pages/index.astro`**

Write `src/pages/index.astro`:
```astro
---
import { getCollection } from 'astro:content';
import BaseHead from '../components/BaseHead.astro';
import Footer from '../components/Footer.astro';
import Header from '../components/Header.astro';
import PostListItem from '../components/PostListItem.astro';
import { SITE_DESCRIPTION, SITE_TITLE } from '../consts';

const posts = (await getCollection('blog'))
	.sort((a, b) => b.data.pubDate.valueOf() - a.data.pubDate.valueOf())
	.slice(0, 5);
---

<!doctype html>
<html lang="en">
	<head>
		<BaseHead title={SITE_TITLE} description={SITE_DESCRIPTION} />
	</head>
	<body>
		<Header />
		<main>
			<section class="intro">
				<p>
					I'm {SITE_TITLE} — a software engineer. I write about the things I build,
					the tools I use, and problems worth remembering how I solved.
				</p>
			</section>
			<section>
				<h2>Recent posts</h2>
				<ul class="post-list">
					{posts.map((post) => <PostListItem post={post} />)}
				</ul>
				<p class="all-link"><a href="/blog">All posts →</a></p>
			</section>
		</main>
		<Footer />
	</body>
</html>

<style>
	.intro p {
		font-size: 1.15rem;
		color: var(--color-text-muted);
		margin-bottom: var(--space-6);
	}
	.post-list { margin: 0; padding: 0; }
	.all-link {
		margin-top: var(--space-4);
		font-family: var(--font-display);
		font-size: 0.95rem;
	}
	.all-link a { text-decoration: none; }
</style>
```

- [ ] **Step 3: Replace `src/pages/blog/index.astro`**

Write `src/pages/blog/index.astro`:
```astro
---
import { getCollection } from 'astro:content';
import BaseHead from '../../components/BaseHead.astro';
import Footer from '../../components/Footer.astro';
import Header from '../../components/Header.astro';
import PostListItem from '../../components/PostListItem.astro';
import { SITE_DESCRIPTION, SITE_TITLE } from '../../consts';

const posts = (await getCollection('blog')).sort(
	(a, b) => b.data.pubDate.valueOf() - a.data.pubDate.valueOf(),
);
---

<!doctype html>
<html lang="en">
	<head>
		<BaseHead title={`Blog · ${SITE_TITLE}`} description={SITE_DESCRIPTION} />
	</head>
	<body>
		<Header />
		<main>
			<h1>Blog</h1>
			<ul class="post-list">
				{posts.map((post) => <PostListItem post={post} />)}
			</ul>
		</main>
		<Footer />
	</body>
</html>

<style>
	.post-list { margin: var(--space-4) 0 0 0; padding: 0; }
</style>
```

- [ ] **Step 4: Build**

Run:
```bash
npm run build
```
Expected: still FAILS only on `about.astro` (`heroImage`). `index.astro` and `blog/index.astro` must no longer error. If either list page errors, fix before continuing.

- [ ] **Step 5: Commit**

```bash
git add src/components/PostListItem.astro src/pages/index.astro src/pages/blog/index.astro
git commit -m "feat: minimal home intro + post-list home and archive pages"
```

---

### Task 9: About page

**Files:**
- Modify: `src/pages/about.astro`

**Interfaces:**
- Consumes: `BlogPost` layout with `coverImage` prop.

- [ ] **Step 1: Replace `src/pages/about.astro`**

Write `src/pages/about.astro`:
```astro
---
import Layout from '../layouts/BlogPost.astro';
---

<Layout
	title="About"
	description="A little about me and this blog."
	pubDate={new Date('2026-06-21')}
>
	<p>
		Hi — I'm a software engineer. This is where I keep notes on what I build and
		learn. Posts here are mostly technical: tools, patterns, and the occasional
		write-up of a problem that took me too long to solve.
	</p>
	<p>
		This blog is built with <a href="https://astro.build/">Astro</a> and deployed
		on Cloudflare Pages. You can subscribe via the <a href="/rss.xml">RSS feed</a>.
	</p>
	<p>Replace this copy with your own bio.</p>
</Layout>
```

Note: no `coverImage` prop is passed — the About page intentionally has no cover. `BlogPost` already guards `coverImage` with `&&`.

- [ ] **Step 2: Build**

Run:
```bash
npm run build
```
Expected: build behavior now depends only on blog content. It may FAIL because the starter placeholder posts still reference `heroImage` in their frontmatter — fixed in Task 10. If it errors on `about.astro` specifically, fix before continuing.

- [ ] **Step 3: Commit**

```bash
git add src/pages/about.astro
git commit -m "feat: rewrite about page with real copy, no cover"
```

---

### Task 10: Replace content with a real intro post

**Files:**
- Delete: `src/content/blog/first-post.md`, `second-post.md`, `third-post.md`, `markdown-style-guide.md`, `using-mdx.mdx`
- Create: `src/assets/posts/hello-world-cover.jpg`
- Create: `src/content/blog/hello-world.md`

**Interfaces:**
- Consumes: `coverImage` schema field; Shiki code highlighting; `<Image>` optimization.

- [ ] **Step 1: Delete the placeholder posts**

Run:
```bash
git rm "src/content/blog/first-post.md" "src/content/blog/second-post.md" "src/content/blog/third-post.md" "src/content/blog/markdown-style-guide.md" "src/content/blog/using-mdx.mdx"
```

- [ ] **Step 2: Create the cover image asset directory and seed an image**

Copy an existing placeholder so a real optimized image exists (replace later with your own):
```bash
mkdir -p src/assets/posts
cp src/assets/blog-placeholder-2.jpg src/assets/posts/hello-world-cover.jpg
```

- [ ] **Step 3: Create `src/content/blog/hello-world.md`**

Write `src/content/blog/hello-world.md`:
```markdown
---
title: 'Hello, world'
description: 'Why I started this blog, and a quick tour of how it renders code.'
pubDate: 2026-06-21
coverImage: '../../assets/posts/hello-world-cover.jpg'
---

This is the first post on my new blog. It exists mostly to prove the plumbing
works: typography, images, and — most importantly for a dev blog — code.

## Code blocks

Syntax highlighting uses Shiki with a light and a dark theme that follow the
site toggle. Hover a block to reveal a copy button.

```ts
type Post = {
	title: string;
	pubDate: Date;
};

function isRecent(post: Post): boolean {
	const THIRTY_DAYS = 1000 * 60 * 60 * 24 * 30;
	return Date.now() - post.pubDate.valueOf() < THIRTY_DAYS; // [!code highlight]
}
```

You can highlight a line with a `// [!code highlight]` comment, and mark diffs:

```js
function greet(name) {
	console.log('hi ' + name); // [!code --]
	console.log(`hello, ${name}`); // [!code ++]
}
```

## Inline formatting

Inline `code` is styled too, alongside **bold**, _italic_, and
[links](https://astro.build/).

> A blockquote, for when a sentence deserves a little room.

That's the tour. Real posts go here next.
```

- [ ] **Step 4: Build the full site**

Run:
```bash
npm run build
```
Expected: **exit 0**. This is the first task expected to fully succeed. The intro post, home, archive, about, RSS, and sitemap all generate.

- [ ] **Step 5: Dev-check the post rendering**

Run `npm run dev`, open `http://localhost:4321/blog/hello-world/`:
- Cover image renders optimized at top.
- Code blocks show syntax colors; the highlighted line and diff add/remove lines are visually marked.
- Hovering a code block reveals a **Copy** button that copies the code and shows "Copied".
- Toggling the theme recolors code blocks (light/dark) without reload.

- [ ] **Step 6: Commit**

```bash
git add src/content/blog/hello-world.md src/assets/posts/hello-world-cover.jpg
git commit -m "content: replace starter posts with a real intro post"
```

---

### Task 11: 404 page and font cleanup

**Files:**
- Create: `src/pages/404.astro`
- Delete: `src/assets/fonts/atkinson-regular.woff`, `src/assets/fonts/atkinson-bold.woff`

**Interfaces:**
- Consumes: `BaseHead`, `Header`, `Footer`.

- [ ] **Step 1: Create `src/pages/404.astro`**

Write `src/pages/404.astro`:
```astro
---
import BaseHead from '../components/BaseHead.astro';
import Footer from '../components/Footer.astro';
import Header from '../components/Header.astro';
import { SITE_TITLE } from '../consts';
---

<!doctype html>
<html lang="en">
	<head>
		<BaseHead title={`Not found · ${SITE_TITLE}`} description="Page not found." />
	</head>
	<body>
		<Header />
		<main>
			<h1>404</h1>
			<p>That page doesn't exist. Try the <a href="/blog">blog</a> or
			head <a href="/">home</a>.</p>
		</main>
		<Footer />
	</body>
</html>
```

- [ ] **Step 2: Delete the now-unused Atkinson font files**

Run:
```bash
git rm "src/assets/fonts/atkinson-regular.woff" "src/assets/fonts/atkinson-bold.woff"
```

- [ ] **Step 3: Build**

Run:
```bash
npm run build
```
Expected: exit 0. Confirm nothing referenced the Atkinson files (the old `--font-atkinson` was removed in Task 4). If the build complains about a missing font import, search for `atkinson` and remove the stale reference.

- [ ] **Step 4: Commit**

```bash
git add src/pages/404.astro
git commit -m "feat: add 404 page and remove unused Atkinson fonts"
```

---

### Task 12: Cloudflare Pages deploy documentation

**Files:**
- Modify: `README.md`

**Interfaces:** none (documentation only).

- [ ] **Step 1: Replace `README.md` with project + deploy docs**

Write `README.md`:
```markdown
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
```

- [ ] **Step 2: Build one final time**

Run:
```bash
npm run build
```
Expected: exit 0.

- [ ] **Step 3: Commit**

```bash
git add README.md
git commit -m "docs: project usage and Cloudflare Pages deploy steps"
```

---

## Self-Review

**Spec coverage:**
- Minimal/typographic design system → Tasks 1, 2.
- Dark-mode toggle, no flash → Tasks 2, 3, 5.
- Self-hosted fonts → Tasks 1, 4.
- Syntax-highlighted code (dual theme, copy button, line highlight) → Tasks 1, 2, 6, 10.
- Local optimized images + cover image → Tasks 6, 7, 10.
- Pages (home, post, archive, about, 404), RSS, sitemap → Tasks 7, 8, 9, 10, 11 (RSS/sitemap retained from starter, titles updated via `consts`).
- Typed content collections, schema tightened (`coverImage`) → Task 6.
- Static build, Cloudflare Pages Git integration, `*.pages.dev` placeholder → Tasks 1, 12.
- Real intro post, placeholders removed → Task 10.

**Placeholder scan:** The only `TODO` is the intentional `site` URL note in `astro.config.mjs` (Task 1), resolved by README Step 5 after the Pages project exists. No vague "add error handling" steps; all code is shown in full.

**Type consistency:** `coverImage: ImageMetadata | undefined` is defined in Task 6 and consumed consistently in Tasks 7 (`BlogPost` prop), 9 (omitted, guarded by `&&`), and 10 (frontmatter). `PostListItem` `Props { post: CollectionEntry<'blog'> }` defined in Task 8 and used in both list pages. `<ThemeToggle />` (no props) defined Task 3, used Task 5. `--font-display/-body/-mono` names match across config (Task 1), CSS (Task 2), and BaseHead (Task 4).

**Deferred (per spec, not implemented):** portfolio sections, tags/categories, search, comments, analytics, custom domain.
