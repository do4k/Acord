<script lang="ts">
	import { onDestroy } from "svelte";

	let username = "";
	let connected = false;
	let ws: WebSocket | null = null;
	let error = "";
	let messages: Array<
		string | { type: "gif"; data: string; author: string }
	> = [];
	let messageToSend = "";
	let onlineUsers: string[] = [];

	const API_URL = import.meta.env.VITE_API_URL;
	function connect() {
		console.log(
			`Connecting to WebSocket at ${API_URL}chat?username=${encodeURIComponent(username)}`,
		);
		if (!username) {
			error = "Please enter a username.";
			return;
		}
		error = "";
		ws = new WebSocket(
			`${API_URL}chat?username=${encodeURIComponent(username)}`,
		);
		ws.onopen = () => {
			connected = true;
		};
		ws.onerror = (e) => {
			error = "WebSocket connection failed.";
			connected = false;
		};
		ws.onclose = () => {
			connected = false;
		};
		ws.onmessage = (event) => {
			if (event.data.startsWith("__users__:" )) {
				onlineUsers = event.data
					.replace("__users__:", "")
					.split(",")
					.filter(Boolean);

				console.log("New user connected:", onlineUsers);
			} else {
				let msg;
				try {
					msg = JSON.parse(event.data);
					console.log("JSON Received message:", msg);
				} catch {
					msg = event.data;
					console.log("Received message:", msg);
				}
				messages = [...messages, msg];
				setTimeout(() => {
					const chatDiv = document.getElementById("chat-window");
					if (chatDiv) chatDiv.scrollTop = chatDiv.scrollHeight;
				}, 0);
			}
		};
	}

	function handleGifUpload(event: Event) {
		const input = event.target as HTMLInputElement;
		const file = input.files?.[0];
		if (file && file.type === "image/gif") {
			const reader = new FileReader();
			reader.onload = () => {
				if (ws && connected) {
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

	function sendMessage() {
		if (ws && connected && messageToSend.trim()) {
			ws.send(messageToSend);
			messageToSend = "";
		}
	}

	onDestroy(() => {
		ws?.close();
	});
</script>

<main
	style="min-height:100vh; display:flex; flex-direction:column; justify-content:center; align-items:center;"
>
	<h1 style="margin-bottom:2em;">Acord Chat</h1>
	{#if !connected}
		<section
			style="width:100%; max-width:400px; margin:auto; background:#222; border-radius:12px; box-shadow:0 2px 16px #111; padding:2em; display:flex; flex-direction:column; align-items:center; justify-content:center;"
		>
			<label for="username" style="font-weight:bold; color:#f5f5f5;"
				>Username</label
			>
			<input
				id="username"
				bind:value={username}
				placeholder="Enter your username"
				required
				disabled={connected}
				style="width:100%; padding:0.75em; margin-bottom:1em; border-radius:4px; border:1px solid #ccc; background:#181818; color:#f5f5f5;"
				on:keydown={(e) => e.key === "Enter" && connect()}
			/>
			<button
				on:click={connect}
				disabled={connected}
				style="width:100%; padding:0.75em; border-radius:4px; background:#1976d2; color:#fff; border:none; font-size:1em;"
				>Connect</button
			>
			{#if error}
				<div
					style="margin-top:1em; padding:0.5em; background:#ffebee; color:#b71c1c; border-radius:4px; width:100%;"
				>
					{error}
				</div>
			{/if}
		</section>
	{/if}
	{#if connected}
		<section
			style="position:fixed; top:0; left:0; width:100vw; height:100vh; background:#222; color:#f5f5f5; display:flex; flex-direction:row; align-items:stretch;"
		>
			<div
				style="flex:4; display:flex; flex-direction:column; height:100vh;"
			>
				<h3 style="color:#f5f5f5; margin:1em 0 0.5em 1em;">
					Chat Window
				</h3>
				<div
					id="chat-window"
					style="flex:1; overflow-y:auto; background:#181818; border-radius:8px; border:1px solid #333; margin:0 1em 0.5em 1em; color:#f5f5f5; padding:1em; display:flex; flex-direction:column; gap:0.5em;"
				>
					{#each messages as msg, i}
						{#if typeof msg === "object" && msg.type === "gif"}
							<div
								style="align-self:flex-start; max-width:70%; text-align:left;"
							>
								<div
									style="font-weight:bold; color:#1976d2; margin-bottom:2px;"
								>
									{msg.author}
								</div>
								<img
									src={msg.data}
									alt="GIF"
									style="max-width:200px; border-radius:8px; margin-bottom:4px;"
								/>
							</div>
						{:else if typeof msg === "string"}
							{#if msg.startsWith(`${username}: `)}
								{#if i === 0 || (typeof messages[i - 1] === "string" && !(messages[i - 1] as string).startsWith(`${username}: `))}
									<div
										style="align-self:flex-end; max-width:70%; text-align:right;"
									>
										<div
											style="font-weight:bold; color:#fff; margin-bottom:2px;"
										>
											{username}
										</div>
										<div
											style="background:#1976d2; color:#fff; padding:0.5em 1em; border-radius:16px 16px 0 16px; word-break:break-word; overflow-wrap:anywhere;"
										>
											{msg.replace(`${username}: `, "")}
										</div>
									</div>
								{:else}
									<div
										style="align-self:flex-end; max-width:70%; text-align:right;"
									>
										<div
											style="background:#1976d2; color:#fff; padding:0.5em 1em; border-radius:16px 16px 0 16px; word-break:break-word; overflow-wrap:anywhere;"
										>
											{msg.replace(`${username}: `, "")}
										</div>
									</div>
								{/if}
							{:else if msg.includes(": ")}
								{@const author = msg.split(": ")[0]}
								{#if i === 0 || !(messages[i - 1] as string).startsWith(`${author}: `)}
									<div
										style="align-self:flex-start; max-width:70%; text-align:left;"
									>
										<div
											style="font-weight:bold; color:#1976d2; margin-bottom:2px;"
										>
											{author}
										</div>
										<div
											style="background:#333; color:#f5f5f5; padding:0.5em 1em; border-radius:16px 16px 16px 0; word-break:break-word; overflow-wrap:anywhere;"
										>
											{msg
												.split(": ")
												.slice(1)
												.join(": ")}
										</div>
									</div>
								{:else}
									<div
										style="align-self:flex-start; max-width:70%; text-align:left;"
									>
										<div
											style="background:#333; color:#f5f5f5; padding:0.5em 1em; border-radius:16px 16px 16px 0; word-break:break-word; overflow-wrap:anywhere;"
										>
											{msg
												.split(": ")
												.slice(1)
												.join(": ")}
										</div>
									</div>
								{/if}
							{:else}
								<div
									style="align-self:flex-start; max-width:70%; text-align:left;"
								>
									<div
										style="font-weight:bold; color:#f5f5f5; margin-bottom:2px;"
									>
										Unknown
									</div>
									<div
										style="background:#333; color:#f5f5f5; padding:0.5em 1em; border-radius:16px 16px 16px 0; word-break:break-word; overflow-wrap:anywhere;"
									>
										{msg}
									</div>
								</div>
							{/if}
						{/if}
					{/each}
				</div>
				<div
					style="width:100%; display:flex; gap:0.5em; padding:1em; background:#222; border-top:1px solid #333;"
				>
					<input
						bind:value={messageToSend}
						placeholder="Type a message..."
						style="flex:1; padding:0.5em; border-radius:4px; border:1px solid #444; background:#181818; color:#f5f5f5;"
						on:keydown={(e) => e.key === "Enter" && sendMessage()}
					/>
					<button
						on:click={sendMessage}
						disabled={!messageToSend.trim()}
						style="padding:0.5em 1em; border-radius:4px; background:#1976d2; color:#fff; border:none;"
						>Send</button
					>
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
							/></svg
						>
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
			</div>
			<div
				style="flex:1; background:#181818; border-radius:8px; border:1px solid #333; padding:1em; color:#f5f5f5; min-width:100px; height:100vh; overflow-y:auto; display:flex; flex-direction:column;"
			>
				<h4 style="margin-top:0; margin-bottom:0.5em; color:#f5f5f5;">
					Online Users
				</h4>
				<ul style="list-style:none; padding:0; margin:0;">
					{#each onlineUsers as user}
						<li
							style="padding:0.25em 0; color:{user === username
								? '#1976d2'
								: '#f5f5f5'}; font-weight:{user === username
								? 'bold'
								: 'normal'};"
						>
							{user}
						</li>
					{/each}
				</ul>
			</div>
		</section>
	{/if}
</main>
