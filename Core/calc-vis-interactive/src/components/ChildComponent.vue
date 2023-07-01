<template>
    <Bar :data="chartData" :options="chartOptions" />
  </template>
  
  <script>
  // DataPage.vue
  import { Bar } from 'vue-chartjs'
  import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale } from 'chart.js'
  
  ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale)
  
  export default {
    name: 'BarChart',
    components: { Bar },
    props: {
      data: {
        type: Array,
        required: true,
      },
      labelKey: {
        type: String,
        default: 'label'
      },
      valueKey: {
        type: String,
        default: 'value'
      }
    },
    computed: {
      chartData() {
        const labels = this.data.map((data) => data[this.labelKey]);
        const values = this.data.map((data) => data[this.valueKey]);
  
        return {
          labels: labels,
          datasets: [
            {
              label: 'Chart Data',
              data: values
            }
          ]
        };
      },
      chartOptions() {
        return {
          responsive: true,
          maintainAspectRatio: false
        };
      }
    },
    watch: {
      data() {
        // Trigger chart update/re-render when data changes
        this.$nextTick(() => {
          this.renderChart(this.chartData, this.chartOptions);
        });
      }
    },
    mounted() {
      this.renderChart(this.chartData, this.chartOptions);
    }
  }
  </script>
  