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

<style scoped>
.card-header {
  font-size: 1.5rem;
  font-weight: bold;
}
.card {
  /* display: flex; */
  /* justify-content: center; */
  padding: 15px;
  /* border: 1px solid #dddddd; */
  border-radius: 10px;
  /* display: grid; */
  /* justify-items: center; */
  /* align-items: center; */
  text-align: center;
  background-color: rgb(255, 255, 255);
  box-shadow: rgba(0, 0, 0, 0.24) 0px 3px 8px;
}

</style>
