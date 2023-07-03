<template>
  <div>
    <div v-if="data && data.length === 0">
      No Data to Show - Try Reloading the Page
    </div>
    <div v-else>
        <div class="card">
          <h1 class="card-header">by {{ formatTitle(labelKey) }}</h1>
          <Doughnut 
          :data="chartData" 
          :options="chartOptions"
          />
        </div>
      </div>
  </div>
</template>
  
  <script>
  import { Doughnut } from 'vue-chartjs'
  import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js'

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
        chartOptions: {
          responsive: true,
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
                color: '#bfbfbf',
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
                display: false
              },
              ticks: {
                color: '#bfbfbf',
                font: {
                  size: 14,
                },
              },
              grid: {
                display: true,
                color: '#f0f0f0',
              },
            },
          }
        }
      };
    },
    computed: {
      chartData() {
        // log key names to console
        console.log(Object.keys(this.data[0]));
        const processedData = this.preprocessData(this.data);
        const labels = processedData.map((data) => data[this.labelKey]);
        const values = processedData.map((data) => data[this.valueKey]);
        let colors = null
        // get HslColor for each label
        if (this.labelKey === 'buildup_name') {
          colors = labels.map((label) => this.getHslColor(label));
        }
        else {
          colors = this.createHslGradient(labels.length);
        }


        return {
          labels: labels,
          datasets: [
            {
              data: values,
              backgroundColor: colors,
              borderRadius: 5,
            }
          ]
        };
      },
    },
    mounted() {
      console.log(this.data)
    },
    methods: {
      createHslGradient(count) {
      const saturation = 20;
      const lightness = 80;
      // create random start angle between 180 and 400
      const startAngle = Math.floor(Math.random() * 220) + 180;
      const e = 2.718;

      // Sigmoid function
      const sigmoid = x => 1 / (1 + Math.exp(-x));

      // Create upper limit using sigmoid function
      const ePowerCount = Math.pow(e, count);
      let angle = sigmoid(ePowerCount / (ePowerCount + 1));

      // Shift to 0 and stretch to 1
      angle = (angle - 0.5) * 2;

      // Remap to 360 degrees
      angle *= 360 / 2;

      const angleStep = angle / count;

      const hslColors = [];
      for (let i = 0; i < count; i++) {
        const hue = Math.floor(angleStep * i) + startAngle;
        const hslColor = `hsl(${hue}, ${saturation}%, ${lightness}%)`;
        hslColors.push(hslColor);
      }

      return hslColors;
    },

      getHslColor(label) {
        const item = this.data.find((item) => item[this.labelKey] === label);
        const color = item.color;
        const hslColor = `hsl(${color.Hue}, ${color.Saturation + 5}%, ${color.Lightness - 5}%)`;
        console.log(hslColor);
        return hslColor;
      },

      preprocessData(data) {
        const groupedData = {};

        // Group data by key
        data.forEach((item) => {
          const key = item[this.labelKey];
          const value = item[this.valueKey];

          if (Object.prototype.hasOwnProperty.call(groupedData, key)) {
            groupedData[key] += value;
          } else {
            groupedData[key] = value;
          }
        });

        // Convert grouped data into an array of objects
        const processedData = Object.entries(groupedData).map(([key, value]) => ({
          [this.labelKey]: key,
          [this.valueKey]: value
        }));

        return processedData;
      },

      formatTitle(title) {
      // remove underscores and capitalize first letter of each word
      return title.replace(/_/g, ' ').replace(/\w\S*/g, (txt) => txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase());
      }
    }
  };
  </script>
  
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
