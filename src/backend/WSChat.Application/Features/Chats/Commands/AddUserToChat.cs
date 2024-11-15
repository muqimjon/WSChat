namespace WSChat.Application.Features.Chats.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;

public record AddUserToChatCommand(long ChatId, long UserId) : IRequest<Models.ChatUserResponse>;

public class AddUserToChatCommandHandler(IChatDbContext context) : IRequestHandler<AddUserToChatCommand, Models.ChatUserResponse>
{
    public async Task<Models.ChatUserResponse> Handle(AddUserToChatCommand request, CancellationToken cancellationToken)
    {
        var existingChatUser = await context.ChatUsers
            .FirstOrDefaultAsync(cu => cu.ChatId == request.ChatId && cu.UserId == request.UserId, cancellationToken);

        if (existingChatUser is not null)
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
