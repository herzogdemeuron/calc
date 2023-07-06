<template>
  <div class="content">
    <div class="header">
      <h1><span>Calc</span>Live</h1>
      <h2><span>C</span>ost <span>a</span>nd <span>l</span>ca <span>c</span>alculation</h2>
    </div>
    <div v-if="dataset.length === 0" class="no-data">
      No Data - Click on something else
    </div>
    <div v-else class="card-container">
      <div class="card-grid">
        <FactsCard :data="dataset" />
      </div>
      <hr class="line" color="#b2b2b2" width="75%">
      <div class="card-grid">
        <Line_YbyX :data="dataHistory" valueKey="global_warming_potential_a1_a2_a3" title="timeline"/>
        <Bar_YbyX :data="dataset" labelKey="buildup_name" valueKey="global_warming_potential_a1_a2_a3" :sortValue="false" title="Buildup"/>
        <Donut_YbyX :data="dataset" labelKey="group_name" valueKey="global_warming_potential_a1_a2_a3" title="Buildup Group"/>
        <Bar_YbyX :data="dataset" labelKey="material_name" valueKey="global_warming_potential_a1_a2_a3" title="Material"/>
        <Donut_YbyX :data="dataset" labelKey="material_category" valueKey="global_warming_potential_a1_a2_a3" title="Material Category"/>
        <Bar_YbyX :data="dataset" labelKey="element_id" valueKey="global_warming_potential_a1_a2_a3" title="Element"/>
      </div>
      <hr class="line" color="#b2b2b2" width="75%">
      <div class="card-grid">
        <Line_YbyX :data="dataHistory" valueKey="cost" title="timeline"/>
        <Bar_YbyX :data="dataset" labelKey="buildup_name" valueKey="cost" :sortValue="false" title="Buildup"/>
        <Donut_YbyX :data="dataset" labelKey="group_name" valueKey="cost" title="Buildup Group"/>
        <Bar_YbyX :data="dataset" labelKey="material_name" valueKey="cost" title="Material"/>
        <Donut_YbyX :data="dataset" labelKey="material_category" valueKey="cost" title="Material Category"/>
        <Bar_YbyX :data="dataset" labelKey="element_id" valueKey="cost" title="Element"/>
      </div>
    </div>
    <a href="https://github.com/herzogdemeuron/calc" class="link">GitHub</a>
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
    Bar_YbyX,
    FactsCard,
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
      console.log(this.dataset);
      // create history object with timestamp and data
      const time = new Date().toLocaleTimeString();
      // check if dataHistory is longer than 4 entries, if so drop the first entry
      if (this.dataHistory.length > 4) {
        this.dataHistory.shift();
        this.dataHistory.push({ time, data: receivedData });
      } else {
      this.dataHistory.push({ time, data: receivedData });
      }
    }
  }
};
</script>


<style scoped>

.content {
  margin: 0 auto;
  max-width: 1600px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  padding-left: 2rem;
  padding-right: 2rem;
}
.header h1 {
  text-align: left;
  font-size: 2.5rem;
  padding-top: 1rem;
  margin: 0;
  font-weight: 500;
  background: linear-gradient(to right, var(--color-highlight-light), var(--color-highlight-dark));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.header h1 span {
  font-weight: 700;
}

.header h2 {
  text-align: right;
  color: var(--text-sub-color);
}

.header h2 span {
  color: var(--color-highlight-light);
}
.card-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(500px, 1fr));
  grid-gap: 1.5rem;
  padding-top: 1.5rem;
  padding-bottom: 1.5rem;
}

.card-container {
  padding-left: 1rem;
  padding-right: 1rem;
}

.line {
  margin-top: 0;
  margin-bottom: 0;
}


.no-data {
  font-size: 1rem;
  font-weight: bold;
  padding: 2rem;
  color: var(--text-sub-color)
}

.link {
  color: var(--text-sub-color);
  text-decoration: none;
  font-size: 1rem;
  font-weight: normal;
  padding: 2rem;
}
</style>