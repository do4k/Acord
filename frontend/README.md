
# Svelte Chat App

This app asks for a username and connects to the `/chat` websocket. Backend may need updates to accept a username from the frontend.

---

# Svelte Project Setup
Everything you need to build a Svelte project, powered by [`sv`](https://github.com/sveltejs/cli).


## Features
- Username prompt
- WebSocket connection to `/chat`

## Setup
1. Install dependencies (already done)
2. Run the app:
	```sh
	bun run dev --open
	```

---

## Creating a project
If you're seeing this, you've probably already done this step. Congrats!
```sh
# create a new project in the current directory
npx sv create
# create a new project in my-app
npx sv create my-app
```


## Developing
Once you've created a project and installed dependencies, start a development server:
```sh
bun run dev --open
```


## Building
To create a production version of your app:
```sh
bun run build
```
You can preview the production build with `bun run preview`.

> To deploy your app, you may need to install an [adapter](https://svelte.dev/docs/kit/adapters) for your target environment.
