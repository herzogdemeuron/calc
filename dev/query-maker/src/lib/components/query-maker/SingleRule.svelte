<script lang="ts">
    import { createEventDispatcher } from 'svelte';
    import type { SingleRule } from './types';

    export let rule: SingleRule;
    
    const dispatch = createEventDispatcher();

    function updateRule() {
        dispatch('update', rule);
    }
</script>

<div class="flex items-center space-x-2 mt-2">
    <input
        type="text"
        placeholder="Rule name"
        bind:value={rule.name}
        on:input={updateRule}
        class="px-3 py-2 border rounded-md w-48"
    />
    
    <input
        type="text"
        placeholder="Rule value"
        bind:value={rule.value}
        on:input={updateRule}
        class="px-3 py-2 border rounded-md w-48"
    />
    
    <button
        on:click={() => dispatch('delete')}
        class="p-2 text-gray-500 hover:text-gray-700"
    >
        −
    </button>
</div>