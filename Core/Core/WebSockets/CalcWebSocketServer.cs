using Calc.Core.Objects;
using Speckle.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class CalcWebSocketServer
{
    private HttpListener httpListener;
    private CancellationTokenSource cancellationTokenSource;
    private List<WebSocket> connectedSockets;

    public bool IsRunning { get => httpListener.IsListening; }

    public CalcWebSocketServer(string url)
    {
        this.httpListener = new HttpListener();
        this.httpListener.Prefixes.Add(url);
        this.connectedSockets = new List<WebSocket>();
    }

    public async Task Start()
    {
        try
        {
            this.httpListener.Start();
            Debug.WriteLine("WebSocket server is running.");

            this.cancellationTokenSource = new CancellationTokenSource();
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                var context = await this.httpListener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    ProcessWebSocketRequest(context);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"WebSocket server encountered an error: {ex.Message}");
        }
    }

    public async Task Stop()
    {
        this.httpListener.Stop();
        this.cancellationTokenSource.Cancel();

        List<Task> closeTasks = new List<Task>();

        foreach (var socket in connectedSockets)
        {
            closeTasks.Add(socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server is shutting down", CancellationToken.None));
        }

        await Task.WhenAll(closeTasks);

        Debug.WriteLine("WebSocket server stopped.");
    }



    public async Task SendResults(List<Result> results)
    {
        var serializedData = JsonConvert.SerializeObject(results);
        var buffer = Encoding.UTF8.GetBytes(serializedData);
        var segment = new ArraySegment<byte>(buffer);

        foreach (var socket in connectedSockets)
        {
            await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        Debug.WriteLine("Results sent to clients");
    }

    private async void ProcessWebSocketRequest(HttpListenerContext context)
    {
        try
        {
            HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
            WebSocket socket = webSocketContext.WebSocket;

            connectedSockets.Add(socket);

            // Handle the WebSocket connection in a separate task
            Task.Run(async () => await HandleWebSocketConnection(socket));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error while accepting WebSocket connection: {ex.Message}");
        }
    }

    private async Task HandleWebSocketConnection(WebSocket socket)
    {
        byte[] buffer = new byte[1024];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Debug.WriteLine($"Received message: {message}");
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocket connection closed", CancellationToken.None);
                connectedSockets.Remove(socket);
                break;
            }
        }
    }
}
