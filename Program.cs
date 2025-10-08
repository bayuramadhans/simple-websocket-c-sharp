using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Store active WebSocket connections
var connections = new ConcurrentDictionary<string, WebSocket>();

// Enable serving static files from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// WebSocket endpoint
app.Map("/ws", async (HttpContext context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var connectionId = Guid.NewGuid().ToString();
        connections.TryAdd(connectionId, webSocket);

        Console.WriteLine($"Client connected: {connectionId}");
        
        // Send welcome message
        await BroadcastMessage("System", $"User {connectionId[..8]} joined the chat", connections);

        await HandleWebSocketConnection(webSocket, connectionId);

        // Cleanup
        connections.TryRemove(connectionId, out _);
        await BroadcastMessage("System", $"User {connectionId[..8]} left the chat", connections);
        Console.WriteLine($"Client disconnected: {connectionId}");
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.UseWebSockets();
app.Run();

async Task HandleWebSocketConnection(WebSocket webSocket, string connectionId)
{
    var buffer = new byte[1024 * 4];

    try
    {
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), 
                CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received from {connectionId[..8]}: {message}");

                // Broadcast to all connected clients
                await BroadcastMessage(connectionId[..8], message, connections);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Closing",
                    CancellationToken.None);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

async Task BroadcastMessage(string sender, string message, ConcurrentDictionary<string, WebSocket> connections)
{
    var messageObj = new
    {
        sender = sender,
        message = message,
        timestamp = DateTime.UtcNow
    };

    var json = JsonSerializer.Serialize(messageObj);
    var bytes = Encoding.UTF8.GetBytes(json);

    var tasks = connections.Values
        .Where(ws => ws.State == WebSocketState.Open)
        .Select(async ws =>
        {
            try
            {
                await ws.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error broadcasting: {ex.Message}");
            }
        });

    await Task.WhenAll(tasks);
}