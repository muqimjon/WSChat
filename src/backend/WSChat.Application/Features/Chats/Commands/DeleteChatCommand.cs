namespace WSChat.Application.Features.Chats.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Features.Chats.Models;
using WSChat.Application.Interfaces;

public record DeleteChatCommand(long ChatId) : IRequest<ChatResponse>;

public class DeleteChatCommandHandler(IChatDbContext context) : IRequestHandler<DeleteChatCommand, ChatResponse>
{
    public async Task<ChatResponse> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await context.Chats
            .Include(c => c.ChatUsers)
            .FirstOrDefaultAsync(c => c.Id == request.ChatId, cancellationToken);

        if (chat == null)
            return new ChatResponse
            {
                Success = false,
                Message = "Chat topilmadi"
            };

        context.ChatUsers.RemoveRange(chat.ChatUsers);
        context.Chats.Remove(chat);
        await context.SaveChangesAsync(cancellationToken);

        return new ChatResponse
        {
            Success = true,
            Message = "Chat muvaffaqiyatli o'chirildi"
        };
    }
}
