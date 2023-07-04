<template>
  <div class="card">
    <h1 class="card-header">{{ cardTitle }}</h1>
    <Bar class="chart" :data="chartData" :options="chartOptions"/>
  </div>
</template>
  
<script>
  import { Bar } from 'vue-chartjs'
  import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale } from 'chart.js'
  import { getChartData } from '../utils/data.js';
  import { formatTitle } from '../utils/text.js';

  ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale)
  
  export default {
    name: 'Bar_YbyX',
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
      },
      sortValue: {
        type: Boolean,
        default: true
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
              display: false
            },
          },
          scales: {
            x: {
              border: {
                display: false
              },
              ticks: {
                callback: function(value) {
                  const label =  this.getLabelForValue(value)
                  if (label.length > 10) {
                     return label.substr(0, 10) + '...';
                  }
                  return label;
                },
                color: '#323232',
                font: {
                  size: 14,
                },
              },
              grid: {
                display: false,
              },
            },
            y: {
              border: {
                display: false,
                dash: [1, 4],
              },
              ticks: {
                color: '#d9dbda',
                font: {
                  size: 14,
                },
              },
              grid: {
                display: true,
                color: '#d9dbda',
                drawTicks: false,
              },
            },
          }
        }
      };
    },
    computed: {
      chartData() {
        const params =  getChartData(this.data, this.labelKey, this.valueKey, this.sortValue)
        return {
          labels: params.labels,
          datasets: [
            {
              data: params.values,
              backgroundColor: params.colors,
              borderSkipped: false,
              borderRadius: 10,
            },
          ],
        }
      },
    },
  };
</script>
  