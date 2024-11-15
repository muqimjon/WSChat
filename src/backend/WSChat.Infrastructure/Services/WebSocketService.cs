namespace WSChat.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;

public class WebSocketService(WebSocketManager webSocketManager, IChatDbContext dbContext) : IWebSocketService
{
    public async Task HandleWebSocketAsync(string username, WebSocket socket, CancellationToken cancellationToken = default)
    {
        webSocketManager.AddConnection(username, socket);
        var buffer = new byte[1024 * 4];

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var message = JsonConvert.DeserializeObject<Message>(messageJson);

                    if (message != null)
                    {
                        await SendMessageToChatMembersAsync(message, cancellationToken);
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    webSocketManager.RemoveConnection(username);
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket error: {ex.Message}");
            webSocketManager.RemoveConnection(username);
        }
    }

    public async Task SendMessageToChatMembersAsync(Message message, CancellationToken cancellationToken = default)
    {
        var chat = await dbContext.Chats
            .Include(c => c.ChatUsers)
            .ThenInclude(cu => cu.User)
            .FirstOrDefaultAsync(c => message.ChatId == c.Id, cancellationToken) ?? new();

        var members = chat.ChatUsers.Select(cu => cu.User.Username).ToList();
        var sockets = webSocketManager.GetConnections(members);

        var fileUrl = string.Empty;

        if (!string.IsNullOrEmpty(message.FilePath))
            fileUrl = message.FilePath;

        var replyToMessageId = message.ReplyToMessageId ?? 0;

        var messageJson = JsonConvert.SerializeObject(new
        {
            message.Content,
            message.SenderId,
            message.ChatId,
            ReplyToMessageId = replyToMessageId,
            SentDate = message.CreatedDate,
            FileUrl = fileUrl,
        });

        var messageBytes = Encoding.UTF8.GetBytes(messageJson);
        var buffer = new ArraySegment<byte>(messageBytes);

        foreach (var socket in sockets)
        {
            try
            {
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }
    }

}

public class WebSocketManager
{
    private readonly ConcurrentDictionary<string, WebSocket> connections = new();

    public bool AddConnection(string usernames, WebSocket socket)
        => connections.TryAdd(usernames, socket);

    public bool RemoveConnection(string usernames)
        => connections.TryRemove(usernames, out _);

    public WebSocket? GetConnection(string usernames)
    {
        connections.TryGetValue(usernames, out var socket);
        return socket;
    }

    public IEnumerable<WebSocket> GetConnections(IEnumerable<string> usernames)
    {
        foreach (var userId in usernames)
            if (connections.TryGetValue(userId, out var socket) && socket.State == WebSocketState.Open)
                yield return socket;
    }
}
