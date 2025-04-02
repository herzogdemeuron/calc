<script lang="ts">
    import RuleBuilder from '$lib/components/query-maker/RuleBuilder.svelte';
    import type { Rule } from '$lib/components/query-maker/types';
    
    let rules: Rule[] = [];

    function handleUpdate(event: CustomEvent<{rules: Rule[]}>) {
        rules = event.detail.rules;
        console.log('Updated rules:', rules);
    }
</script>

<RuleBuilder {rules} on:update={handleUpdate} />