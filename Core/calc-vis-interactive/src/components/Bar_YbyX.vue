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
          responsive: true
        }
      };
    },
    computed: {
      chartData() {
        const processedData = this.preprocessData(this.data);
        const labels = processedData.map((data) => data[this.labelKey]);
        const values = processedData.map((data) => data[this.valueKey]);

        return {
          labels: labels,
          datasets: [
            {
              data: values,
            }
          ]
        };
      },
    },
    mounted() {
      console.log(this.data)
    },
    methods: {
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
  