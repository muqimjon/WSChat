namespace WSChat.Application.Interfaces;

using System.Net.WebSockets;
using System.Threading;
using WSChat.Application.Features.Messaging.Models;

public interface IWebSocketService
{
    Task HandleWebSocketAsync(long userId, WebSocket socket, CancellationToken cancellation);
    Task SendMessageToChatMembersAsync(MessageResultDto message, CancellationToken cancellationToken);
}
