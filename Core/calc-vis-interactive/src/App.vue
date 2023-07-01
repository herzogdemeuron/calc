<template>
  <div>
    <p>Parent Component</p>
    <ChildComponent :data="dataset" labelKey="buildup_name" valueKey="global_warming_potential_a1_a2_a3" />
  </div>
</template>

<script>
import ChildComponent from './/components/ChildComponent.vue';
import { reactive } from 'vue';

export default {
  components: {
    ChildComponent
  },
  data() {
    return {
      dataset: reactive([]),
      socket: null
    };
  },
  mounted() {
    this.socket = new WebSocket('ws://127.0.0.1:8184');
    this.socket.onopen = () => {
      alert("[open] Connection established");
      alert("Sending to server");
      this.socket.send("Vue app says hello");
    };

    this.socket.onmessage = (event) => {
      this.handleWebSocketMessage(event.data);
    };

    this.socket.onclose = (event) => {
      if (event.wasClean) {
        alert(`[close] Connection closed cleanly, code=${event.code} reason=${event.reason}`);
      } else {
        alert('[close] Connection died');
      }
    };

    this.socket.onerror = (error) => {
      alert(`WebSocket Error: ${error.message}`);
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
