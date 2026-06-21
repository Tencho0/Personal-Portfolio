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
