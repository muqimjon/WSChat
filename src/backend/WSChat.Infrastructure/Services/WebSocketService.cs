namespace WSChat.Infrastructure.Services;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using WSChat.Application.Exceptions;
using WSChat.Application.Features.Messaging.Models;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;

public class WebSocketService(
    WebSocketManager webSocketManager,
    IChatDbContext context,
    IMapper mapper,
    IMediator mediator) : IWebSocketService
{
    public async Task HandleWebSocketAsync(long userId, WebSocket socket, CancellationToken cancellationToken = default)
    {
        webSocketManager.AddConnection(userId, socket);
        var buffer = new byte[1024 * 4];

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var message = JsonConvert.DeserializeObject<Message>(messageJson);

                    if (message is not null)
                    {
                        await context.Messages.AddAsync(message, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);

                        var messageDto = mapper.Map<MessageResultDto>(message);
                        await SendMessageToChatMembersAsync(messageDto, cancellationToken);
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    webSocketManager.RemoveConnection(userId);
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"WebSocket connection for {userId} was canceled.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket error: {ex.Message}");
            webSocketManager.RemoveConnection(userId);
        }
        finally
        {
            if (socket.State != WebSocketState.Closed)
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closing", cancellationToken);
        }
    }


    public async Task SendMessageToChatMembersAsync(MessageResultDto message, CancellationToken cancellationToken = default)
    {
        var chat = await context.Chats
            .Include(c => c.ChatUsers)
            .ThenInclude(cu => cu.User)
            .FirstOrDefaultAsync(c => message.Chat.Id == c.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Chat), nameof(Chat.Id), message.Chat.Id);

        var messageJson = JsonConvert.SerializeObject(message);
        var members = chat.ChatUsers.Select(cu => cu.User.Id).ToList();
        var messageBytes = Encoding.UTF8.GetBytes(messageJson);
        var buffer = new ArraySegment<byte>(messageBytes);
        var sockets = webSocketManager.GetConnections(members);

        foreach (var socket in sockets)
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
}
