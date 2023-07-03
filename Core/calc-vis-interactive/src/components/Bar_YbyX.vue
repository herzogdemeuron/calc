<template>
  <div>
    <div v-if="data && data.length === 0">
      No Data to Show - Try Reloading the Page
    </div>
    <div v-else>
      <Bar 
        :data="chartData" 
        :options="chartOptions"
      />
    </div>
  </div>
</template>
  
  <script>
  import { Bar } from 'vue-chartjs'
  import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale } from 'chart.js'

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
              backgroundColor: colors
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
      const saturation = 40;
      const lightness = 40;
      const startAngle = 200;
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
        const hslColor = `hsl(${color.Hue}, ${color.Saturation}%, ${color.Lightness}%)`;
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
      }
    }
  };
  </script>
  