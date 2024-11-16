namespace WSChat.Application.Features.Chats.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Exceptions;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;

public record RemoveUserFromChatCommand(long ChatId, long UserId) : IRequest<bool>;

public class RemoveUserFromChatCommandHandler(IChatDbContext context) :
    IRequestHandler<RemoveUserFromChatCommand, bool>
{
    public async Task<bool> Handle(RemoveUserFromChatCommand request, CancellationToken cancellationToken)
    {
        _ = await context.Chats.FirstOrDefaultAsync(cu => cu.Id == request.ChatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Chat), nameof(Chat.Id), request.ChatId);

        _ = await context.Users.FirstOrDefaultAsync(cu => cu.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), nameof(User.Id), request.UserId);

        var chatUser = await context.ChatUsers
            .FirstOrDefaultAsync(cu => cu.UserId == request.UserId && cu.ChatId == request.ChatId, cancellationToken)
            ?? throw new CustomException("The user does not belong to this chat.");

        context.ChatUsers.Remove(chatUser);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
