namespace WSChat.Application.Interfaces;

using System.Net.WebSockets;
using System.Threading;
using WSChat.Domain.Entities;

public interface IWebSocketService
{
    Task HandleWebSocketAsync(string username, WebSocket socket, CancellationToken cancellation);
    Task SendMessageToChatMembersAsync(Message message, CancellationToken cancellationToken);
}
