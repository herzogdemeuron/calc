<template>
  <div class="card">
    <h1 class="card-header">{{ cardTitle }}</h1>
    <Doughnut class="chart" :data="chartData" :options="chartOptions"/>
  </div>
</template>
  
<script>
  import { Doughnut } from 'vue-chartjs'
  import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js'
  import { getChartData } from '../utils/data.js';
  import { formatTitle } from '../utils/text.js';


  ChartJS.register(ArcElement, Tooltip, Legend)
  
  export default {
    name: 'Donut_YbyX',
    components: { Doughnut },
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
    data() {
      return {
        cardTitle: formatTitle(this.labelKey),
        chartOptions: {
          responsive: true,
          aspectRatio: 1.5,
          plugins: {
            legend: {
              display: true,
              position: 'bottom',
              labels: {
                color: '#323232',
                font: {
                  size: 14,
                },
              },
            },
          },
        }
      };
    },
    computed: {
      chartData() {
        return getChartData(this.data, this.labelKey, this.valueKey)
      },
    },
  };
</script>
  