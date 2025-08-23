<script lang="ts">
	import { onDestroy } from "svelte";
	import OnlineList from "../components/OnlineList.svelte";
	import ChatInput from "../components/ChatInput.svelte";
	import MessageDisplay from "../components/MessageDisplay.svelte";
	import Login from "../components/Login.svelte";

	let username = "";
	let connected = false;
	let ws: WebSocket | null = null;
	let error = "";
	let messages: Array<
		string | { type: "gif"; data: string; author: string }
	> = [];
	let onlineUsers: string[] = [];
	// Mobile: we'll show a full-width users row beneath the input instead of a floating button

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
			if (event.data.startsWith("__users__:")) {
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

	onDestroy(() => {
		ws?.close();
	});
</script>

<style>
	.mobile-users-row { display: none; }
	/* Responsive layout helpers */
	@media (max-width: 700px) {
		.chat-section {
			flex-direction: column !important;
		}
		.online-panel {
			display: none !important;
		}
		/* Mobile-only inline users row */
		.mobile-users-row {
			display: block;
			padding: 0.5rem 1rem 1rem;
			background: #222;
			border-top: 1px solid #333;
		}
		.user-row-inner { display: flex; align-items: center; gap: 0.5rem; }
		.chip-scroller { display: flex; gap: 0.5rem; overflow-x: auto; }
		.chip {
			background: #181818;
			border: 1px solid #333;
			color: #f5f5f5;
			padding: 0.25rem 0.5rem;
			border-radius: 999px;
			white-space: nowrap;
			font-size: 0.85rem;
		}
		.chip.me { background: #1976d2; border-color: #1976d2; color: #fff; }
	}
</style>

<main
	style="min-height:100vh; display:flex; flex-direction:column; justify-content:center; align-items:center;"
>
	<h1 style="margin-bottom:2em;">Acord Chat</h1>
	{#if !connected}
		<section style="width:100%; max-width:480px; margin:auto; padding:1rem; display:flex; flex-direction:column; align-items:stretch; justify-content:center;">
			<Login
				{username}
				{connected}
				{error}
				onUsernameInput={(inputValue) => (username = inputValue)}
				onConnect={() => connect()}
			/>
		</section>
	{:else}
		<section class="chat-section" style="position:fixed; top:0; left:0; width:100vw; height:100vh; background:#222; color:#f5f5f5; display:flex; flex-direction:row; align-items:stretch;">
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
					<MessageDisplay {messages} {username} />
				</div>
				<div
					style="width:100%; display:flex; gap:0.5em; padding:1em; background:#222; border-top:1px solid #333;"
				>
					<ChatInput {ws} {username} />
				</div>
				<!-- Mobile-only users row below the input -->
				<div class="mobile-users-row">
					<div class="user-row-inner">
						<strong>Online:</strong>
						<div class="chip-scroller">
							{#each onlineUsers as user}
								<span class="chip" class:me={user === username}>{user}</span>
							{/each}
						</div>
					</div>
				</div>
			</div>
			<div class="online-panel"
				style="flex:1; background:#181818; border-radius:8px; border:1px solid #333; padding:1em; color:#f5f5f5; min-width:100px; height:100vh; overflow-y:auto; display:flex; flex-direction:column;"
			>
				<OnlineList users={onlineUsers} currentUser={username} />
			</div>
		</section>
	{/if}
</main>
