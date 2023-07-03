<template>
  <div class="app">
    <h1 class="header">CALC Live Visualization</h1>
    <div class="grid-container">
      <div class="grid-item">
        <div class="chart-grid">
          <TotalGWP :data="dataset" valueKey="global_warming_potential_a1_a2_a3" />
          <Bar_YbyX
            v-for="(chart, index) in barChartList"
            :key="index"
            :data="dataset"
            :labelKey="chart.labelKey"
            :valueKey="chart.valueKey"
          />
        </div>
      </div>
      <div class="grid-item">
        <div class="chart-grid">
          <Donut_YbyX
            v-for="(chart, index) in donutChartList"
            :key="index"
            :data="dataset"
            :labelKey="chart.labelKey"
            :valueKey="chart.valueKey"
          />
        </div>
      </div>
    </div>
  </div>
</template>


<script>
import Bar_YbyX from './components/Bar_YbyX.vue';
import TotalGWP from './components/TotalGWP.vue';
import Donut_YbyX from './components/Donut_YbyX.vue';
import { reactive } from 'vue';

export default {
  components: {
    TotalGWP,
    Bar_YbyX,
    Donut_YbyX
  },
  data() {
    return {
      dataset: reactive([]),
      socket: null,
      barChartList: [
        { labelKey: 'buildup_name', valueKey: 'global_warming_potential_a1_a2_a3' },
        { labelKey: 'element_id', valueKey: 'global_warming_potential_a1_a2_a3' }
      ],
      donutChartList: [
        { labelKey: 'group_name', valueKey: 'global_warming_potential_a1_a2_a3' },
        { labelKey: 'material_category', valueKey: 'global_warming_potential_a1_a2_a3' }
      ]
    };
  },
  mounted() {
    this.connectWebSocket();
  },
  methods: {
    connectWebSocket() {
  if (this.socket !== null && this.socket.readyState === WebSocket.OPEN) {
    // WebSocket is already open, no need to reconnect
    return;
  }

  this.socket = new WebSocket('ws://127.0.0.1:8184');

  this.socket.onopen = () => {
    let time = new Date().toLocaleTimeString();
    this.socket.send(`${time} cal-viz-interactive connected`);
  };

  this.socket.onmessage = (event) => {
    console.log('Message received from server');
    this.handleWebSocketMessage(event.data);
  };

  this.socket.onclose = (event) => {
    if (event.wasClean) {
      console.log(`Connection closed cleanly, code=${event.code} reason=${event.reason}`);
    } else {
      console.log('Connection died');
    }

    // Attempt to reconnect after a delay (e.g., 5 seconds)
    setTimeout(() => {
      this.connectWebSocket();
    }, 5000);
  };

  this.socket.onerror = (error) => {
    console.log(`WebSocket Error: ${error.message}`);
    // Handle the error as needed
  };

  // Add event listener for beforeunload event
  window.addEventListener('beforeunload', () => {
    if (this.socket.readyState === WebSocket.OPEN) {
      // WebSocket is still open, send disconnection message to the server
      this.socket.send('calc-viz-interactive disconnected');
      this.socket.close();
    }
  });
},

handleWebSocketMessage(data) {
  // Assuming the received data is in JSON format
  const receivedData = JSON.parse(data);
  this.dataset = reactive(receivedData);
}

  }
};
</script>

<style scoped>

.grid-container {
  display: flex;
  grid-template-columns: 1fr 1fr 1fr; /* Three equal-width columns */
  grid-gap: 20px; /* Spacing between columns */
  padding: 20px;
  justify-content: center;
}

.chart-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(700px, 1fr));
  grid-gap: 20px;
  text-align: center;
}

.header {
  text-align: center;
  font-size: 2rem;
  font-weight: bold;
  margin-bottom: 10px;
  margin-top: 20px;
}
</style>