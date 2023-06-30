using Calc.Core.Objects;

public class Program
{
    static CalcWebSocketServer server;

    static async Task Main(string[] args)
    {
        server = new CalcWebSocketServer("http://127.0.0.1:8184/"); // Update the URL to use "http://" instead of "ws://"

        Task serverTask = server.Start(); // Start the server asynchronously

        string command;
        do
        {
            command = Console.ReadLine();

            // Example: Trigger sending data to clients when a specific command is entered
            if (command == "send")
            {
                await server.SendResults(new List<Result>()
                {
                    new Result() { GlobalWarmingPotentialA1A2A3 = 10, GroupName = "Group 1" },
                    new Result() { GlobalWarmingPotentialA1A2A3 = 20, GroupName = "Group 2" },
                    new Result() { GlobalWarmingPotentialA1A2A3 = 30, GroupName = "Group 3" },
                });
            }
        }
        while (command != "exit");

        server.Stop();
        await serverTask; // Wait for the server to stop gracefully
    }
}
