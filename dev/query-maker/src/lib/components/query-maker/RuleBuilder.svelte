<script lang="ts">
    import { createEventDispatcher } from 'svelte';
    import type { Rule, GroupRule } from './types';
    import RuleGroup from './RuleGroup.svelte';

    export let rules: Rule[] = [];
    
    const dispatch = createEventDispatcher();
    
    const rootGroup: GroupRule = {
        id: 'root',
        type: 'group',
        operator: 'and',
        rules
    };

    function handleUpdate(event: CustomEvent<GroupRule>) {
        rules = event.detail.rules;
        dispatch('update', { rules });
    }
</script>

<div class="w-full max-w-4xl mx-auto p-4">
    <div class="bg-white rounded-lg shadow">
        <div class="p-4 border-b">
            <h2 class="text-lg font-semibold">Rule Builder</h2>
        </div>
        <div class="p-4">
            <RuleGroup 
                group={rootGroup}
                depth={0}
                on:update={handleUpdate}
            />
        </div>
    </div>
</div>