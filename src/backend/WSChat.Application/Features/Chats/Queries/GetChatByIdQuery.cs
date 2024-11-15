namespace WSChat.Application.Features.Chats.Queries;

using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Features.Chats.Models;
using WSChat.Application.Interfaces;

public record GetChatByIdQuery(long ChatId) : IRequest<ChatDetailsResponse>;

public class GetChatByIdQueryHandler(IChatDbContext context) : IRequestHandler<GetChatByIdQuery, ChatDetailsResponse>
{
    public async Task<ChatDetailsResponse> Handle(GetChatByIdQuery request, CancellationToken cancellationToken)
    {
        var chat = await context.Chats
            .Include(ch => ch.ChatUsers)
            .ThenInclude(cu => cu.User)
            .Include(ch => ch.Creator)
            .Include(ch => ch.Messages)
            .FirstOrDefaultAsync(ch => ch.Id == request.ChatId, cancellationToken);

        if (chat is null)
            return new ChatDetailsResponse();

        var chatResponse = new ChatDetailsResponse
        {
            ChatId = chat.Id,
            ChatName = chat.ChatName,
            Creator = new UserResponse
            {
                UserId = chat.CreatorId,
                Username = chat.Creator.Username,
                Name = chat.Creator.Name,                
            },
            Members = chat.ChatUsers.Select(ch => new UserResponse
            {
                UserId = ch.User.Id,
                Username = ch.User.Username,
                Name = ch.User.Name,
            }).ToList(),
            Messages = chat.Messages.Select(m => new MessageResponse
            {
                Id = m.Id,
                Content = m.Content,
                FilePath = m.FilePath,
                ReplyToMessage = m.ReplyToMessage is null ? null : new()
                {
                    Content = m.ReplyToMessage?.Content,
                    Id = m.ReplyToMessage!.Id,
                    Status = m.ReplyToMessage!.Status,
                    FilePath = m.ReplyToMessage.FilePath,
                },
                Status = m.Status,
            }).ToList(),
        };

        return chatResponse;
    }
}
