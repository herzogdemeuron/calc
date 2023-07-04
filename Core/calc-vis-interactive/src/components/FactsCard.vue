<template>
<div class="card">
    <h1 class="card-header">Quick Facts</h1>
    <div class="grid-container">
        <div class="grid-item">{{ total }}</div>
        <div class="grid-item">kgCO2e (Gwp A123)</div>
        <div class="grid-item">{{ buildupCount }}</div>
        <div class="grid-item">Buildups</div>
        <div class="grid-item">{{ elementCount }}</div>
        <div class="grid-item">Elements</div>
        <div class="grid-item">{{ groupCount }}</div>
        <div class="grid-item">Groups</div>
        <div class="grid-item">{{ materialCount }}</div>
        <div class="grid-item">Materials</div>
    </div>
</div>
</template>

<script>
export default {
    name: 'FactsCard',
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
        },
        buildupCount() {
            // count unique buildups
            const buildups = new Set();
            this.data.forEach((item) => {
                buildups.add(item.buildup_name);
            });
            return buildups.size;
        },
        elementCount() {
            // count unique element_id
            const elements = new Set();
            this.data.forEach((item) => {
                elements.add(item.element_id);
            });
            return elements.size;
        },
        groupCount() {
            // count unique group_id
            const groups = new Set();
            this.data.forEach((item) => {
                groups.add(item.group_name);
            });
            return groups.size;
        },
        materialCount() {
            // count unique material_id
            const materials = new Set();
            this.data.forEach((item) => {
                materials.add(item.material_name);
            });
            return materials.size;
        },
    }
}
</script>

<style scoped>
.grid-container {
  display: grid;
  grid-template-columns: 10% auto; /* Set the width of the columns */
  grid-gap: 10px; /* Adjust the gap between grid items as per your preference */
  margin-left: 1.5rem;
  align-items: baseline;
}

.grid-item {
  text-align: right; /* Align items in the first column to the right */
  font-size: 1.5rem;
  font-weight: bold;
}

.grid-item:nth-child(2n) {
    text-align: left; /* Align items in the second column to the left */
    font-size: 1rem;
    color: #868686;
    font-weight: lighter;
}
</style>
