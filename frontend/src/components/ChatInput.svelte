<script lang="ts">
  export let ws: WebSocket | null;
  export let username: string;

  let messageToSend: string = "";

	function sendMessage() {
		if (ws && ws.readyState === WebSocket.OPEN && messageToSend.trim()) {
			ws.send(messageToSend);
			messageToSend = "";
		}
	}

	function handleGifUpload(event: Event) {
		const input = event.target as HTMLInputElement;
		const file = input.files?.[0];
		if (file && file.type === "image/gif") {
			const reader = new FileReader();
			reader.onload = () => {
				if (ws && ws.readyState === WebSocket.OPEN) {
					ws.send(
						JSON.stringify({
							type: "gif",
							data: reader.result,
							author: username,
						}),
					);
				}
			};
			reader.readAsDataURL(file);
		}
	}

  function handlePaste(event: ClipboardEvent) {
		console.log("Paste event detected");
		if (!ws) {
			console.error("WebSocket is not initialized.");
			return;
		}
		if (ws.readyState !== WebSocket.OPEN) {
			console.error("WebSocket is not connected.");
			return;
		}
		const items = event.clipboardData?.items;
		if (!items) {
			console.error("No clipboard items found.");
			return;
		}
		for (const item of items) {
			console.log("Clipboard item:", item);
			if (item.type === "image/gif") {
				const file = item.getAsFile();
				if (!file) {
					console.error("Failed to get GIF file.");
					continue;
				}
				const reader = new FileReader();
				reader.onload = () => {
					if (ws) {
						console.log("Sending GIF:", reader.result);
						ws.send(
							JSON.stringify({
								type: "gif",
								data: reader.result,
								author: username,
							})
						);
					} else {
						console.error("WebSocket is not connected.");
					}
				};
				reader.readAsDataURL(file);
				event.preventDefault();
				break;
			}
		}
  }
</script>

<div style="width:100%; display:flex; gap:0.5em; align-items:center;">
  <input
    bind:value={messageToSend}
    placeholder="Type a message..."
    style="flex:1; padding:0.5em; border-radius:4px; border:1px solid #444; background:#181818; color:#f5f5f5;"
    on:keydown={(e) => e.key === 'Enter' && sendMessage()}
    on:paste={handlePaste}
  />
  <button
    on:click={sendMessage}
    disabled={!messageToSend.trim()}
    style="padding:0.5em 1em; border-radius:4px; background:#1976d2; color:#fff; border:none;"
  >Send</button>
  <label
    for="gif-upload"
    style="margin-left:1em; cursor:pointer; background:#1976d2; color:#fff; padding:0.5em 1em; border-radius:4px; font-size:1em; display:flex; align-items:center; gap:0.5em;"
  >
    <svg
      xmlns="http://www.w3.org/2000/svg"
      width="20"
      height="20"
      fill="currentColor"
      viewBox="0 0 20 20"
    ><path
      d="M10 2a6 6 0 0 0-6 6v3.586l-.293.293A1 1 0 0 0 4 14h12a1 1 0 0 0 .707-1.707l-.293-.293V8a6 6 0 0 0-6-6zm-4 6a4 4 0 1 1 8 0v4H6V8zm-1 6v-1h10v1H5z"
    /></svg>
    Upload GIF
    <input
      id="gif-upload"
      type="file"
      accept="image/gif"
      on:change={handleGifUpload}
      style="display:none;"
    />
  </label>
</div>
