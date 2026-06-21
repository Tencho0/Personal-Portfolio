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
