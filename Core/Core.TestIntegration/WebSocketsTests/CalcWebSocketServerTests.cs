
using Calc.Core.Objects;
using Calc.Core.WebSockets;
using Speckle.Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.TestIntegration.WebSocketsTests
{
    [TestClass]
    public class CalcWebSocketServerTests
    {
        [TestMethod]
        public async Task OnResultsUpdated_EventRaised_SendsUpdatedDataToClient()
        {
            // Arrange
            var server = new CalcWebSocketServer("ws://127.0.0.1:8080");
            List<Result> updatedResults = new()
            {
                new Result { Id = 1, MaterialName = "Test" },
                new Result { Id = 2, MaterialName = "Test2" }
            };

            List<string> sentMessages = new List<string>();

            // Subscribe to the event and capture the messages sent to the client
            ResultManager.OnResultsUpdated += (results) =>
            {
                var json = JsonConvert.SerializeObject(results);
                sentMessages.Add(json);
            };

            // Act
            ResultManager.UpdateData(updatedResults);

            // Allow some time for the event to be handled and the message to be sent
            await Task.Delay(100);

            // Assert
            Assert.AreEqual(1, sentMessages.Count);
            var expectedJson = JsonConvert.SerializeObject(updatedResults);
            Assert.AreEqual(expectedJson, sentMessages[0]);
            // pretty print expectedJson
            Console.WriteLine(JsonConvert.SerializeObject(updatedResults, Formatting.Indented));
        }
    }
}
