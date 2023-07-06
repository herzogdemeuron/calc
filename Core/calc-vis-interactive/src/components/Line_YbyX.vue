<template>
    <div class="card">
      <h1 class="card-header">{{ cardTitle }} <span> {{ cardSubtitle }}</span></h1>
      <Line ref="line" class="chart" :data="chartData" :options="chartOptions"/>
    </div>
  </template>
    
  <script>
    import { getChartHistoryData } from '../utils/data.js';
    import { formatTitle } from '../utils/text.js';
    import { Line } from 'vue-chartjs'
    import {
        Chart as ChartJS,
        Title,
        Tooltip,
        Legend,
        LineElement,
        CategoryScale,
        LinearScale,
        PointElement,
        // Plugin,
        Filler,
        // BorderRadius
        } from 'chart.js'

    ChartJS.register(Title, Tooltip, Legend, LineElement, LinearScale, CategoryScale, PointElement, Filler)

    export default {
      name: 'Line_YbyX',
      components: { Line },
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
                display: false
              },
            },
            scales: {
              x: {
                border: {
                  display: false
                },
                ticks: {
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
                  font: {
                    size: 14,
                  },
                },
                grid: {
                  display: true,
                  drawTicks: false,
                },
              },
            }
          }
        };
      },
      mounted() { 
        this.$nextTick(() => {
          var chartInstance = this.$refs.line.chart;
          var x = chartInstance.config.options.scales.x;
          var y = chartInstance.config.options.scales.y;

          var style = getComputedStyle(document.body);
          var chartScaleColor = style.getPropertyValue('--chart-scale-color');

          x.grid.color = chartScaleColor;
          y.grid.color = chartScaleColor;
          x.ticks.color = chartScaleColor;
          y.ticks.color = chartScaleColor;
        });
      },
      computed: {
        chartData() {
          const params = getChartHistoryData(this.data, this.valueKey)
          return {
            labels: params.labels,
            datasets: [
            {
                data: params.snapshotTotals,
                label: 'Total',
                fill: true,
                backgroundColor: (ctx) => {
                    const canvas = ctx.chart.ctx;
                    const gradient = canvas.createLinearGradient(0,0,0,180);

                    gradient.addColorStop(0, 'rgba(112, 122, 194, 0.7)');
                    gradient.addColorStop(1, 'rgba(112, 194, 180, 0.2)');

                    return gradient;
                },
                borderColor: 'rgba(106, 117, 200, 1)', // Set the line color
                tension: 0.3,
            }
            ]
        }
        },
      },
    };
  </script>

  
    