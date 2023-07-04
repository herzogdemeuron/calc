<template>
<div class="card">
    <h2 class="card-header">Total GWP A123</h2>
    <h1 class="total"> {{ total }} kgCO2e</h1>
</div>
</template>

<script>
export default {
    name: 'TotalGWP',
    props: {
        data: {
            type: Array,
            required: true,
        },
        valueKey: {
            type: String,
            default: 'value'
        }
    },
    computed: {
        total() {
            return this.data.reduce((acc, cur) => acc + cur[this.valueKey], 0);
        }
    }
}
</script>

<style scoped>
.total {
    text-align: center;
}
</style>
