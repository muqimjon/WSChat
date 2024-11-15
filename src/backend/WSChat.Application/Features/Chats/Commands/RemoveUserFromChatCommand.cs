namespace WSChat.Application.Features.Chats.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Features.Chats.Models;
using WSChat.Application.Interfaces;

public record RemoveUserFromChatCommand(long ChatId, long UserId) : IRequest<ChatUserResponse>;

public class RemoveUserFromChatCommandHandler(IChatDbContext context) : IRequestHandler<RemoveUserFromChatCommand, ChatUserResponse>
{
    public async Task<ChatUserResponse> Handle(RemoveUserFromChatCommand request, CancellationToken cancellationToken)
    {
        var chatUser = await context.ChatUsers
            .FirstOrDefaultAsync(cu => cu.ChatId == request.ChatId && cu.UserId == request.UserId, cancellationToken);

        if (chatUser is null)
            return new ChatUserResponse
            {
                Success = false,
                Message = "Foydalanuvchi chatda topilmadi."
            };


        context.ChatUsers.Remove(chatUser);
        await context.SaveChangesAsync(cancellationToken);

        return new ChatUserResponse
        {
            Success = true,
            Message = "Foydalanuvchi muvaffaqiyatli chatdan chiqarildi.",
            ChatId = request.ChatId,
            UserId = request.UserId
        };
    }
}
