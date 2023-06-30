using System;
using System.Collections.Generic;
using Speckle.Newtonsoft.Json;
using Fleck;
using Calc.Core.Objects;

namespace Calc.Core.WebSockets
{
    public class CalcWebSocketServer
    {
        private WebSocketServer server;

        public CalcWebSocketServer(string url)
        {
            server = new WebSocketServer(url);
            server.Start(HandleWebSocketConnection);
        }

        private void HandleWebSocketConnection(IWebSocketConnection connection)
        {
            // Handle incoming WebSocket connections
            connection.OnOpen = () =>
            {
                // Perform any initialization tasks when a new connection is established
                Console.WriteLine("WebSocket connection opened");
            };

            connection.OnClose = () =>
            {
                // Handle WebSocket connection close event
                Console.WriteLine("WebSocket connection closed");
            };

            // Other WebSocket event handlers can be added as needed

            // Example: Send initial data to the client when the connection is established
            connection.Send("Welcome to the WebSocket server!");

            // Example: Subscribe to the OnDataUpdated event
            ResultManager.OnResultsUpdated += (results) =>
            {
                // Send updated data to the client whenever the OnDataUpdated event is raised
                var json = JsonConvert.SerializeObject(results);
                connection.Send(json);
            };
        }
    }

    public class ResultManager
    {
        public static event Action<List<Result>> OnResultsUpdated;
        private static List<Result> Results;

        public static void UpdateData(List<Result> results)
        {
            // Update the data as needed
            Results = results;

            // Example: Raise the OnDataUpdated event
            OnResultsUpdated?.Invoke(Results);
        }
    }
}