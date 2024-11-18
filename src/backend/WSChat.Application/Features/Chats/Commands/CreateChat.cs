namespace WSChat.Application.Features.Chats.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Exceptions;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;
using WSChat.Domain.Enums;

public record CreateChatCommand(string ChatName, long CreatorId, long ParticipantId) : IRequest<long>;

public class CreateChatCommandHandler(
    IChatDbContext context,
    IMapper mapper) :
    IRequestHandler<CreateChatCommand, long>
{
    public async Task<long> Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == request.CreatorId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), nameof(User.Id), request.CreatorId);

        var participant = await context.Users.FirstOrDefaultAsync(u => u.Id == request.ParticipantId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), nameof(User.Id), request.ParticipantId);

        var chat = mapper.Map<Chat>(request);
        await context.Chats.AddAsync(chat, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var chatUserOwner = new ChatUser
        {
            ChatId = chat.Id,
            UserId = request.CreatorId,
            Role = ChatUserRole.Owner,
        };

        await context.ChatUsers.AddAsync(chatUserOwner, cancellationToken);

        var chatUserParticipant = new ChatUser
        {
            ChatId = chat.Id,
            UserId = request.ParticipantId,
            Role = ChatUserRole.User,
        };

        await context.ChatUsers.AddAsync(chatUserParticipant, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return chat.Id;
    }
}
