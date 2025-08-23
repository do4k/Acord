<script lang="ts">
  export let users: string[] = [];
  export let currentUser: string = "";
</script>

<h4 style="margin-top:0; margin-bottom:0.5em; color:#f5f5f5;">
    Online Users
</h4>
<ul style="list-style:none; padding:0; margin:0;">
  {#each users as user}
    <li
      style="padding:0.25em 0; color:{user === currentUser ? '#1976d2' : '#f5f5f5'}; font-weight:{user === currentUser ? 'bold' : 'normal'};"
    >
      {user}
    </li>
  {/each}
</ul>
