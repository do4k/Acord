<script lang="ts">
  export let messages: Array<string | { type: "gif"; data: string; author: string }> = [];
  export let username: string = "";
</script>

{#each messages as msg, i}
  {#if typeof msg === "object" && msg.type === "gif"}
    <div
      style="align-self:{msg.author === username ? 'flex-end' : 'flex-start'}; max-width:70%; text-align:{msg.author === username ? 'right' : 'left'};"
    >
      <div
        style="font-weight:bold; color:{msg.author === username ? '#fff' : '#1976d2'}; margin-bottom:2px;"
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
    {#if msg.startsWith("SYSTEM: ")}
      <div style="align-self:center; text-align:center; width:100%;">
        <div style="background:#444; color:#ffd700; padding:0.5em 1em; border-radius:16px; font-weight:bold; margin:0.5em auto; display:inline-block;">
          {msg.replace("SYSTEM: ", "")}
        </div>
      </div>
    {:else if msg.startsWith(`${username}: `)}
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
