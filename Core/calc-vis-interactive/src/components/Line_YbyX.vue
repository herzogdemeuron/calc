<template>
    <div class="card">
      <h1 class="card-header">{{ cardTitle }}</h1>
      <Line id="gwpChart" class="chart" :data="chartData" :options="chartOptions"/>
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
        }
      },
      data() {
        return {
          cardTitle: formatTitle("gwp a123"),
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

                    gradient.addColorStop(0.2, 'rgba(55, 158, 210, 0.72)');
                    gradient.addColorStop(1, 'rgba(112, 194, 194, 0.2)');

                    return gradient;
                },
                borderColor: 'rgba(65, 133, 200, 1)', // Set the line color
                tension: 0.3,
            }
            ]
        }
        },
      },
    };
  </script>
    