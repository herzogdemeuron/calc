<template>
  <div class="content">
    <h1 class="header">Calc Live</h1>
    <div v-if="dataset.length === 0" class="card">
      No Data - Send again
    </div>
    <div v-else class="card-grid">
      <FactsCard :data="dataset" />
      <Line_YbyX :data="dataHistory" valueKey="global_warming_potential_a1_a2_a3" />
      <Bar_YbyX :data="dataset" labelKey="buildup_name" valueKey="global_warming_potential_a1_a2_a3" sortValue="false"/>
      <Donut_YbyX :data="dataset" labelKey="group_name" valueKey="global_warming_potential_a1_a2_a3" />
      <Bar_YbyX :data="dataset" labelKey="material_name" valueKey="global_warming_potential_a1_a2_a3" />
      <Donut_YbyX :data="dataset" labelKey="material_category" valueKey="global_warming_potential_a1_a2_a3" />
    </div>
  </div>
</template>


<script>
import Bar_YbyX from './components/Bar_YbyX.vue';
import FactsCard from './components/FactsCard.vue';
import Donut_YbyX from './components/Donut_YbyX.vue';
import Line_YbyX from './components/Line_YbyX.vue';
import { reactive } from 'vue';

export default {
  components: {
    FactsCard,
    Bar_YbyX,
    Donut_YbyX,
    Line_YbyX
  },
  data() {
    return {
      dataset: reactive([]),
      dataHistory: reactive([]),
      socket: null
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
  // create history object with timestamp and data
  const time = new Date().toLocaleTimeString();
  this.dataHistory.push({ time, data: receivedData });
}

  }
};
</script>

<style scoped>

.content {
  margin: 1rem auto;
  max-width: 1600px;
  padding-bottom: 2rem;
}
.card-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(500px, 1fr));
  grid-gap: 1.5rem;
  text-align: center;
}

.header {
  text-align: left;
  font-size: 2.5rem;
  font-weight: bold;
  margin: 1rem;
  background: linear-gradient(to right, #6cc, #254993);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}
</style>