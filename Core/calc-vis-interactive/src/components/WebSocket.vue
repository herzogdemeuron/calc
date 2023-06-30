<template>
    <div>
      <label for="message">Message:</label>
      <p>{{ message }}</p>
    </div>
  </template>
  
  <script>
  export default {
    data() {
        return {
            message: ''
        };
    },
    mounted() {
    // Create a WebSocket connection
    let socket = new WebSocket('ws://127.0.0.1:8184');
    let vm = this;
  
    // Event handler for when the WebSocket connection is established
    socket.onopen = function() {
        alert("[open] Connection established");
        alert("Sending to server");
        socket.send("Vue app says hello");
    };

    socket.onmessage = function(event) {
    vm.message = event.data; // Assign received data to the 'message' property
    };

    socket.onclose = function(event) {
        if (event.wasClean) {
            alert(`[close] Connection closed cleanly, code=${event.code} reason=${event.reason}`);
        } else {
            // e.g. server process killed or network down
            // event.code is usually 1006 in this case
            alert('[close] Connection died');
        }
    };

    socket.onerror = function(error) {
        alert(`WebSocket Error: ${error.message}`);
    };
    }
  };
  </script>
  