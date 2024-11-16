namespace WSChat.Infrastructure.Services;
using System.Collections.Concurrent;
using System.Net.WebSockets;

public class WebSocketManager
{
    private readonly ConcurrentDictionary<long, WebSocket> connections = new();

    public bool AddConnection(long userIds, WebSocket socket)
        => connections.TryAdd(userIds, socket);

    public bool RemoveConnection(long userIds)
        => connections.TryRemove(userIds, out _);

    public IEnumerable<WebSocket> GetConnections(IEnumerable<long> userIds)
    {
        foreach (var userId in userIds)
            if (connections.TryGetValue(userId, out var socket) && socket.State == WebSocketState.Open)
                yield return socket;
    }

    public WebSocket? GetConnection(long userId)
    {
        if (connections.TryGetValue(userId, out var socket) && socket.State == WebSocketState.Open)
            return socket;

        return null;
    }
}
