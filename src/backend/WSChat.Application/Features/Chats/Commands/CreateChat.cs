namespace WSChat.Application.Features.Chats.Commands;

using MediatR;
using WSChat.Application.Features.Chats.Models;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;
using WSChat.Domain.Enums;

public record CreateChatCommand(string ChatName, long CreatorId) : IRequest<ChatUserResponse>;

public class CreateChatCommandHandler(IChatDbContext context) : IRequestHandler<CreateChatCommand, ChatUserResponse>
{
    public async Task<ChatUserResponse> Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        var chat = new Chat
        {
            ChatName = request.ChatName,
            ChatType = ChatType.Private,
            CreatorId = request.CreatorId,
        };

        await context.Chats.AddAsync(chat, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var chatUser = new ChatUser
        {
            ChatId = chat.Id,
            UserId = request.CreatorId
        };

        await context.ChatUsers.AddAsync(chatUser, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new ChatUserResponse
        {
            ChatId = chat.Id,
            ChatName = chat.ChatName,
            Message = "Muvaffaqiyatli yaratildi. Id: " + chat.Id,
            Success = true,
            UserId = request.CreatorId
        };
    }
}
