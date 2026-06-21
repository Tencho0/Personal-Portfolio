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
