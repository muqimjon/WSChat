namespace WSChat.Application.Features.Chats.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;
using WSChat.Domain.Enums;

public record AddUserToChatCommand(long ChatId, long UserId) : IRequest<Models.ChatUserResponse>;

public class AddUserToChatCommandHandler(IChatDbContext context) : IRequestHandler<AddUserToChatCommand, Models.ChatUserResponse>
{
    public async Task<Models.ChatUserResponse> Handle(AddUserToChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await context.Chats
            .Include(ch => ch.ChatUsers)
            .FirstOrDefaultAsync(cu => cu.Id == request.ChatId, cancellationToken);

        if (chat is null)
            return new Models.ChatUserResponse
            {
                Success = false,
                Message = "Chat mavjud emas."
            };

        var existingChatUser = chat.ChatUsers.Any(cu => cu.UserId == request.UserId);

        if (existingChatUser)
            return new Models.ChatUserResponse
            {
                Success = false,
                Message = "Foydalanuvchi allaqachon chatda mavjud."
            };

        var newChatUser = new ChatUser
        {
            UserId = request.UserId,
            ChatId = request.ChatId
        };

        if (chat.ChatUsers.Count >= 2)
            chat.ChatType = ChatType.Group;

        await context.ChatUsers.AddAsync(newChatUser, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new Models.ChatUserResponse
        {
            Success = true,
            Message = "Foydalanuvchi muvaffaqiyatli chatga qo'shildi.",
            ChatId = request.ChatId,
            UserId = request.UserId
        };
    }
}
