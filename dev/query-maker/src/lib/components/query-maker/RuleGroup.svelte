<script lang="ts">
    import { createEventDispatcher } from 'svelte';
    import type { GroupRule, Rule } from './types';
    import SingleRule from './SingleRule.svelte';

    export let group: GroupRule;
    export let depth: number = 0;

    const dispatch = createEventDispatcher();

    function addRule() {
        group.rules = [...group.rules, {
            id: crypto.randomUUID(),
            type: 'single',
            name: '',
            value: ''
        }];
        updateGroup();
    }

    function addGroup() {
        group.rules = [...group.rules, {
            id: crypto.randomUUID(),
            type: 'group',
            operator: 'and',
            rules: []
        }];
        updateGroup();
    }

    function updateRule(index: number, updatedRule: Rule) {
        group.rules[index] = updatedRule;
        updateGroup();
    }

    function deleteRule(index: number) {
        group.rules = group.rules.filter((_, i) => i !== index);
        updateGroup();
    }

    function updateGroup() {
        dispatch('update', group);
    }
</script>

<div class="mt-2 border rounded-lg p-4 {depth > 0 ? 'border-gray-200' : ''}">
    <div class="flex items-center space-x-2">
        <select
            bind:value={group.operator}
            on:change={updateGroup}
            class="px-3 py-2 border rounded-md"
        >
            <option value="and">AND</option>
            <option value="or">OR</option>
        </select>

        <button
            on:click={addRule}
            class="px-4 py-2 text-sm bg-blue-500 text-white rounded-md hover:bg-blue-600"
        >
            Add Rule
        </button>

        <button
            on:click={addGroup}
            class="px-4 py-2 text-sm bg-blue-500 text-white rounded-md hover:bg-blue-600"
        >
            Add Group
        </button>

        {#if depth > 0}
            <button
                on:click={() => dispatch('delete')}
                class="p-2 text-gray-500 hover:text-gray-700"
            >
                −
            </button>
        {/if}
    </div>

    <div class="pl-4 {depth > 0 ? 'border-l-2 border-gray-200' : ''} mt-2">
        {#each group.rules as rule, index}
            <div>
                {#if rule.type === 'single'}
                    <SingleRule
                        {rule}
                        on:update={e => updateRule(index, e.detail)}
                        on:delete={() => deleteRule(index)}
                    />
                {:else}
                    <svelte:self
                        group={rule}
                        depth={depth + 1}
                        on:update={e => updateRule(index, e.detail)}
                        on:delete={() => deleteRule(index)}
                    />
                {/if}
            </div>
        {/each}
    </div>
</div>