using Calc.Core.Objects;
using System.Diagnostics;

public class Program
{
    static CalcWebSocketServer server;

    static async Task Main(string[] args)
    {
        // Add ConsoleTraceListener to redirect Debug.WriteLine output to the console
        Trace.Listeners.Add(new ConsoleTraceListener());

        server = new CalcWebSocketServer("http://127.0.0.1:8184/"); // Update the URL to use "http://" instead of "ws://"

        Task serverTask = server.Start(); // Start the server asynchronously
            var sendCounter = 0;

        string command;
        do
        {
            command = Console.ReadLine();

            // Example: Trigger sending data to clients when a specific command is entered
            if (command == "send")
            {
                await server.SendResults(new List<Result>()
                {
                    new Result() { Gwp = 10 + sendCounter, BuildupName = "Buildup 1", GroupName = "Group 1", Color = new Calc.Core.Color.HslColor(80, 40, 60) },
                    new Result() { Gwp = 20 + sendCounter, BuildupName = "Buildup 2", GroupName = "Group 2", Color = new Calc.Core.Color.HslColor(90, 40, 60) },
                    new Result() { Gwp = 30 + sendCounter, BuildupName = "Buildup 3", GroupName = "Group 2", Color = new Calc.Core.Color.HslColor(100, 40, 60) }
                });
                sendCounter++;
            }
        }
        while (command != "exit");

        server.Stop();
        await serverTask; // Wait for the server to stop gracefully
    }
}
