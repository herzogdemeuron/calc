<template>
<div class="card">
    <div class="grid-container">
    <div>
        <h1 class="fact">{{ totalGwp }} </h1>
        <span>GwpA123 (KgCO2e)</span>
    </div>
    <div>
        <h1 class="fact">{{ buildupCount }} </h1>
        <span>Buildups</span>
    </div>
    <div>
        <h1 class="fact">{{ groupCount }} </h1>
        <span>Buildup Groups</span>
    </div>
    <div>
        <h1 class="fact">{{ totalCost }} </h1>
        <span>Cost</span>
    </div>
    <div>
        <h1 class="fact">{{ materialCount }} </h1>
        <span>Materials</span>
    </div>
    <div>
        <h1 class="fact">{{ materialCategoryCount }} </h1>
        <span>Material Categories</span>
    </div>

    </div>
</div>
</template>

<script>
import { formatNumber } from '@/utils/text';

export default {
    name: 'FactsCard',
    props: {
        data: {
            type: Array,
            required: true,
        }
    },
    computed: {
        totalGwp() {
            const gwp = this.data.reduce((acc, cur) => acc + cur.global_warming_potential_a1_a2_a3, 0);
            return formatNumber(Math.round(gwp));
        },
        totalCost() {
            const cost = this.data.reduce((acc, cur) => acc + cur.cost, 0);
            return formatNumber(Math.round(cost));
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
  grid-template-columns: 1fr 1fr 1fr;
  grid-gap: 0.5rem;
  padding: 1.5rem;
}

.fact {
    font-size: 1.5rem;
    font-weight: bold;
    color: #323232;
    margin: 0;
  }
  
  span {
    font-size: 1rem;
    color: #ababab;
  }

</style>
