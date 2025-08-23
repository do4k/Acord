<script lang="ts">
  export let username: string = "";
  export let connected: boolean = false;
  export let error: string = "";
  export let onUsernameInput: (value: string) => void;
  export let onConnect: () => void;
</script>

<div style="width:100%; display:flex; flex-direction:column; align-items:center; justify-content:center;">
  <label for="username" style="font-weight:bold; color:#f5f5f5;">Username</label>
  <input
    id="username"
    bind:value={username}
    placeholder="Enter your username"
    required
    disabled={connected}
    style="width:100%; padding:0.75em; margin-bottom:1em; border-radius:4px; border:1px solid #ccc; background:#181818; color:#f5f5f5;"
    on:input={(e) => onUsernameInput((e.target as HTMLInputElement).value)}
    on:keydown={(e) => e.key === 'Enter' && onConnect()}
  />
  <button
    on:click={onConnect}
    disabled={connected}
    style="width:100%; padding:0.75em; border-radius:4px; background:#1976d2; color:#fff; border:none; font-size:1em;"
  >Connect</button>
  {#if error}
    <div
      style="margin-top:1em; padding:0.5em; background:#ffebee; color:#b71c1c; border-radius:4px; width:100%;"
    >
      {error}
    </div>
  {/if}
</div>
