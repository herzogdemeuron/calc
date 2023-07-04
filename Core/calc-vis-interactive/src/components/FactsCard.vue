<template>
<div class="card">
    <h1 class="card-header">Quick Facts</h1>
    <div class="grid-container">
        <div class="grid-item">{{ total }} </div>
        <div class="grid-item">Gwp A123 (kgCO2e)</div>
        <div class="grid-item">{{ buildupCount }}</div>
        <div class="grid-item">Buildups</div>
        <div class="grid-item">{{ groupCount }}</div>
        <div class="grid-item">Groups (Buildups)</div>
        <div class="grid-item">{{ materialCount }}</div>
        <div class="grid-item">Materials</div>
        <div class="grid-item">{{ materialCategoryCount }}</div>
        <div class="grid-item">Material Categories</div>
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
        materialCategoryCount() {
            // count unique material_category_id
            const materialCategories = new Set();
            this.data.forEach((item) => {
                materialCategories.add(item.material_category);
            });
            return materialCategories.size;
        }
    }
}
</script>

<style scoped>
.grid-container {
  display: grid;
  grid-template-columns: 0.6fr 1fr; /* Set the width of the columns */
  grid-gap: 10px; /* Adjust the gap between grid items as per your preference */
  margin: 3rem;
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
    color: #b2b2b2;
}

</style>
