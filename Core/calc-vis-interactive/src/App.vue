<template>
  <div>
    <p>Parent Component</p>
    <Bar_YbyX :data="dataset" labelKey="buildup_name" valueKey="global_warming_potential_a1_a2_a3" />
    <Bar_YbyX :data="dataset" labelKey="group_name" valueKey="global_warming_potential_a1_a2_a3" />
  </div>
</template>

<script>
import Bar_YbyX from './/components/Bar_YbyX.vue';
import { reactive } from 'vue';

export default {
  components: {
    Bar_YbyX
  },
  data() {
    return {
      // dataset: reactive([]),
      dataset: reactive([]),
      socket: null
    };
  },
  mounted() {
   if (this.socket == null) {
     this.socket = new WebSocket('ws://127.0.0.1:8184')
   }

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
    };

    this.socket.onerror = (error) => {
      console.log(`WebSocket Error: ${error.message}`);
    };
  },
  methods: {
    handleWebSocketMessage(data) {
      // Assuming the received data is in JSON format
      const receivedData = JSON.parse(data);
      this.dataset = reactive(receivedData);
    }
  }
};
</script>
