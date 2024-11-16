namespace WSChat.Infrastructure.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using WSChat.Application.Features.Messaging.Models;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;

public class WebSocketService(
    WebSocketManager webSocketManager,
    IChatDbContext context,
    IMapper mapper) :
    IWebSocketService
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
                        var chat = await ValidateChatAsync(message.ChatId, cancellationToken);
                        var replyMessage = await ValidateReplyMessageAsync(message.ReplyToMessageId, message.ChatId, cancellationToken);
                        var user = await ValidateUserAsync(message.SenderId, cancellationToken);

                        if (chat is null || user is null)
                        {
                            var errorMessage = new ErrorMessageDto
                            {
                                Error = "Chat yoki foydalanuvchi topilmadi."
                            };
                            await SendErrorMessageToUserAsync(userId, errorMessage, cancellationToken);
                            continue;
                        }

                        await context.Messages.AddAsync(message, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);

                        message.Chat = chat;
                        message.Sender = user;
                        message.ReplyToMessage = replyMessage;
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
            var errorMessage = new ErrorMessageDto
            {
                Error = "Server xatosi: " + ex.Message
            };
            await SendErrorMessageToUserAsync(userId, errorMessage, cancellationToken);
        }
        finally
        {
            if (socket.State != WebSocketState.Closed)
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closing", cancellationToken);
        }
    }

    private async Task<Chat?> ValidateChatAsync(long chatId, CancellationToken cancellationToken)
    {
        var chat = await context.Chats.FirstOrDefaultAsync(ch => ch.Id == chatId, cancellationToken);
        if (chat is null)
            Console.WriteLine($"Chat with ID {chatId} not found.");

        return chat;
    }

    private async Task<Message?> ValidateReplyMessageAsync(long? replyToMessageId, long chatId, CancellationToken cancellationToken)
    {
        if (!replyToMessageId.HasValue)
            return null;

        var replyMessage = await context.Messages
            .FirstOrDefaultAsync(m => m.Id == replyToMessageId.Value, cancellationToken);

        if (replyMessage != null && replyMessage.ChatId != chatId)
        {
            Console.WriteLine("Reply message is in another chat.");
            return null;
        }

        return replyMessage;
    }

    private async Task<User?> ValidateUserAsync(long userId, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
            Console.WriteLine($"User with ID {userId} not found.");

        return user;
    }

    public async Task SendMessageToChatMembersAsync(MessageResultDto message, CancellationToken cancellationToken = default)
    {
        var chat = await context.Chats
            .Include(c => c.ChatUsers)
            .ThenInclude(cu => cu.User)
            .FirstOrDefaultAsync(c => message.Chat.Id == c.Id, cancellationToken);

        if (chat is null)
        {
            Console.WriteLine($"Chat is not found | from websocket. {message.Chat.Id}");
            return;
        }

        var messageJson = JsonConvert.SerializeObject(message);
        var members = chat.ChatUsers.Select(cu => cu.User.Id).ToList();
        var messageBytes = Encoding.UTF8.GetBytes(messageJson);
        var buffer = new ArraySegment<byte>(messageBytes);
        var sockets = webSocketManager.GetConnections(members);

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

    public async Task SendMessageToUserAsync(long userId, MessageResultDto message, CancellationToken cancellationToken = default)
    {
        var userSocket = webSocketManager.GetConnection(userId);

        if (userSocket is null)
        {
            Console.WriteLine($"No active WebSocket connection for user {userId}.");
            return;
        }

        try
        {
            var messageJson = JsonConvert.SerializeObject(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageJson);
            var buffer = new ArraySegment<byte>(messageBytes);

            await userSocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message to user {userId}: {ex.Message}");
        }
    }

    public async Task SendErrorMessageToUserAsync(long userId, ErrorMessageDto errorMessage, CancellationToken cancellationToken = default)
    {
        var userSocket = webSocketManager.GetConnection(userId);

        if (userSocket is null)
        {
            Console.WriteLine($"No active WebSocket connection for user {userId}.");
            return;
        }

        try
        {
            var errorJson = JsonConvert.SerializeObject(errorMessage);
            var errorBytes = Encoding.UTF8.GetBytes(errorJson);
            var buffer = new ArraySegment<byte>(errorBytes);

            await userSocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending error message to user {userId}: {ex.Message}");
        }
    }
}

public class ErrorMessageDto
{
    public string Error { get; set; }
}
