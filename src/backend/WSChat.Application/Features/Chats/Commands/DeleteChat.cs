namespace WSChat.Application.Features.Chats.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Exceptions;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;

public record DeleteChatCommand(long ChatId) : IRequest<bool>;

public class DeleteChatCommandHandler(IChatDbContext context) : IRequestHandler<DeleteChatCommand, bool>
{
    public async Task<bool> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await context.Chats
            .Include(c => c.ChatUsers)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == request.ChatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Chat), nameof(Chat.Id), request.ChatId);

        context.ChatUsers.RemoveRange(chat.ChatUsers);
        context.Messages.RemoveRange(chat.Messages);
        context.Chats.Remove(chat);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
