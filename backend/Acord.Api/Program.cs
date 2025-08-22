using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();
app.UseWebSockets();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "Acord API is running.");

var userClients = new ConcurrentDictionary<string, WebSocket>();
app.Map("/chat", async (HttpContext context, [FromServices] ILogger<Program> logger, [FromQuery] string username) =>
{
    logger.LogInformation("Starting WebSocket connection for user: {Username}", username);
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = 400;
        logger.LogError("WebSocket request expected but not received.");
        return;
    }

    if (string.IsNullOrWhiteSpace(username))
    {
        context.Response.StatusCode = 400;
        logger.LogError("Username is required for WebSocket connection.");
        await context.Response.WriteAsync("Username is required");
        return;
    }

    if (userClients.ContainsKey(username) && userClients[username].State == WebSocketState.Open)
    {
        context.Response.StatusCode = 409;
        logger.LogError("Username '{Username}' is already connected.", username);
        await context.Response.WriteAsync("Username is already connected");
        return;
    }

    var ws = await context.WebSockets.AcceptWebSocketAsync();
    userClients[username] = ws;
    var buffer = new byte[1024 * 4];

    await BroadcastUserList(userClients);

    try
    {
        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                logger.LogInformation("WebSocket connection closed by client: {Username}", username);
                break;
            }
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            string broadcastMessage;
            if (message.StartsWith("{") && message.Contains("\"type\":\"gif\"")) {
                // Forward GIF JSON as-is
                broadcastMessage = message;
            } else {
                // Regular text message
                broadcastMessage = $"{username}: {message}";
            }
            foreach (var client in userClients.Values)
            {
                if (client.State == WebSocketState.Open)
                {
                    logger.LogInformation("Broadcasting message: {Message}", broadcastMessage);
                    await client.SendAsync(Encoding.UTF8.GetBytes(broadcastMessage), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
    finally
    {
        userClients.TryRemove(username, out _);
        await BroadcastUserList(userClients);
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the WebSocket handler", CancellationToken.None);
    }
});

static async Task BroadcastUserList(ConcurrentDictionary<string, WebSocket> userClients)
{
    var userList = string.Join(",", userClients.Keys);
    var userMsg = $"__users__:{userList}";
    var bytes = Encoding.UTF8.GetBytes(userMsg);
    foreach (var ws in userClients.Values)
    {
        if (ws.State == WebSocketState.Open)
        {
            await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}

app.Run();