<template>
  <div class="card">
    <h1 class="card-header">{{ cardTitle }} <span> {{ cardSubtitle }}</span></h1>
    <Doughnut ref="donut" class="chart" :data="chartData" :options="chartOptions"/>
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
      },
      title: {
        type: String,
        default: 'Title'
      },
    },
    data() {
      return {
        cardTitle: formatTitle(this.title),
        cardSubtitle: formatTitle(this.valueKey),
        chartOptions: {
          responsive: true,
          aspectRatio: 1.5,
          plugins: {
            legend: {
              display: true,
              position: 'bottom',
              labels: {
                font: {
                  size: 14,
                },
              },
            },
          },
        }
      };
    },
    mounted() { 
        this.$nextTick(() => {
          var chartInstance = this.$refs.donut.chart;
          var style = getComputedStyle(document.body);
          var chartScaleColor = style.getPropertyValue('--chart-scale-color');
          
          chartInstance.config.options.plugins.legend.labels.color = chartScaleColor;
        });
      },
      computed: {
        chartData() {
          var style = getComputedStyle(document.body);
          var cardColor = style.getPropertyValue('--card-background-color');
          
          const params =  getChartData(this.data, this.labelKey, this.valueKey, true)
        return {
          labels: params.labels,
          datasets: [
            {
              data: params.values,
              backgroundColor: params.colors,
              borderRadius: 10,
              borderWidth: 3,
              borderColor: cardColor,
            },
          ],
        }
      },
    },
  };
</script>
  