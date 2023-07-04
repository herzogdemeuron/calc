<template>
    <div class="card">
      <h1 class="card-header">{{ cardTitle }}</h1>
      <Line class="chart" :data="chartData" :options="chartOptions"/>
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
          return getChartHistoryData(this.data, this.valueKey)
        },
      },
    };
  </script>
    