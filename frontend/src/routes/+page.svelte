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

<main
	style="min-height:100vh; display:flex; flex-direction:column; justify-content:center; align-items:center;"
>
	<h1 style="margin-bottom:2em;">Acord Chat</h1>
	{#if !connected}
		<section
			style="width:100%; max-width:400px; margin:auto; background:#222; border-radius:12px; box-shadow:0 2px 16px #111; padding:2em; display:flex; flex-direction:column; align-items:center; justify-content:center;"
		>
			<Login
				{username}
				{connected}
				{error}
				onUsernameInput={(inputValue) => (username = inputValue)}
				onConnect={() => connect()}
			/>
		</section>
	{:else}
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
					<MessageDisplay {messages} {username} />
				</div>
				<div
					style="width:100%; display:flex; gap:0.5em; padding:1em; background:#222; border-top:1px solid #333;"
				>
					<ChatInput {ws} {username} />
				</div>
			</div>
			<div
				style="flex:1; background:#181818; border-radius:8px; border:1px solid #333; padding:1em; color:#f5f5f5; min-width:100px; height:100vh; overflow-y:auto; display:flex; flex-direction:column;"
			>
				<OnlineList users={onlineUsers} currentUser={username} />
			</div>
		</section>
	{/if}
</main>
