

<script lang="ts">
	import { onDestroy } from 'svelte';

	let username = '';
	let connected = false;
	let ws: WebSocket | null = null;
	let error = '';
	let messages: string[] = [];
	let messageToSend = '';
	let onlineUsers: string[] = [];

	const API_URL = import.meta.env.VITE_API_URL;
	function connect() {
		console.log(`Connecting to WebSocket at ${API_URL}/chat?username=${encodeURIComponent(username)}`);
		if (!username) {
			error = 'Please enter a username.';
			return;
		}
		error = '';
		ws = new WebSocket(`${API_URL}/chat?username=${encodeURIComponent(username)}`);
		ws.onopen = () => {
			connected = true;
		};
		ws.onerror = (e) => {
			error = 'WebSocket connection failed.';
			connected = false;
		};
		ws.onclose = () => {
			connected = false;
		};
		ws.onmessage = (event) => {
			// Expect backend to send user list as: '__users__:user1,user2,user3'
			if (event.data.startsWith('__users__:')) {
				onlineUsers = event.data.replace('__users__:', '').split(',').filter(Boolean);
			} else {
				messages = [...messages, event.data];
				setTimeout(() => {
					const chatDiv = document.getElementById('chat-window');
					if (chatDiv) chatDiv.scrollTop = chatDiv.scrollHeight;
				}, 0);
			}
		};
	}

	function sendMessage() {
		if (ws && connected && messageToSend.trim()) {
			ws.send(messageToSend);
			messageToSend = '';
		}
	}

	onDestroy(() => {
		ws?.close();
	});
</script>




	<main style="min-height:100vh; display:flex; flex-direction:column; justify-content:center; align-items:center;">
		<h1 style="margin-bottom:2em;">Acord Chat</h1>
		{#if !connected}
		<section style="width:100%; max-width:400px; margin:auto; background:#222; border-radius:12px; box-shadow:0 2px 16px #111; padding:2em; display:flex; flex-direction:column; align-items:center; justify-content:center;">
			<label for="username" style="font-weight:bold; color:#f5f5f5;">Username</label>
			<input id="username" bind:value={username} placeholder="Enter your username" required disabled={connected} style="width:100%; padding:0.75em; margin-bottom:1em; border-radius:4px; border:1px solid #ccc; background:#181818; color:#f5f5f5;"
				on:keydown={(e) => e.key === 'Enter' && connect()} />
			<button on:click={connect} disabled={connected} style="width:100%; padding:0.75em; border-radius:4px; background:#1976d2; color:#fff; border:none; font-size:1em;">Connect</button>
			{#if error}
				<div style="margin-top:1em; padding:0.5em; background:#ffebee; color:#b71c1c; border-radius:4px; width:100%;">{error}</div>
			{/if}
		</section>
		{/if}
		{#if connected}
		<section style="width:100%; max-width:500px; margin:auto; margin-top:2em; padding:1em; background:#222; border-radius:12px; box-shadow:0 2px 16px #111; color:#f5f5f5; display:flex; flex-direction:column; align-items:center;">
			<h3 style="color:#f5f5f5; margin-bottom:1em;">Chat Window</h3>
			<div style="width:100%; display:flex; gap:2em;">
				<div style="flex:2;">
					<div id="chat-window" style="width:100%; height:300px; overflow-y:auto; background:#181818; border-radius:8px; border:1px solid #333; margin-bottom:1em; color:#f5f5f5; padding:1em; display:flex; flex-direction:column; gap:0.5em;">
						{#each messages as msg}
							{#if msg.startsWith(`${username}: `)}
								<div style="align-self:flex-end; background:#1976d2; color:#fff; padding:0.5em 1em; border-radius:16px 16px 0 16px; max-width:70%; text-align:right;">{msg.replace(`${username}: `, '')}</div>
							{:else}
								<div style="align-self:flex-start; background:#333; color:#f5f5f5; padding:0.5em 1em; border-radius:16px 16px 16px 0; max-width:70%; text-align:left;">{msg}</div>
							{/if}
						{/each}
					</div>
					<div style="width:100%; display:flex; gap:0.5em;">
						<input
							bind:value={messageToSend}
							placeholder="Type a message..."
							style="flex:1; padding:0.5em; border-radius:4px; border:1px solid #444; background:#181818; color:#f5f5f5;"
							on:keydown={(e) => e.key === 'Enter' && sendMessage()}
						/>
						<button on:click={sendMessage} disabled={!messageToSend.trim()} style="padding:0.5em 1em; border-radius:4px; background:#1976d2; color:#fff; border:none;">Send</button>
					</div>
				</div>
				<div style="flex:1; background:#181818; border-radius:8px; border:1px solid #333; padding:1em; color:#f5f5f5; min-width:120px;">
					<h4 style="margin-top:0; margin-bottom:0.5em; color:#f5f5f5;">Online Users</h4>
					<ul style="list-style:none; padding:0; margin:0;">
						{#each onlineUsers as user}
							<li style="padding:0.25em 0; color:{user === username ? '#1976d2' : '#f5f5f5'}; font-weight:{user === username ? 'bold' : 'normal'};">{user}</li>
						{/each}
					</ul>
				</div>
			</div>
		</section>
		{/if}
	</main>
