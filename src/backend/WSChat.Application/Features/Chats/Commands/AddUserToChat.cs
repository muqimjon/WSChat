namespace WSChat.Application.Features.Chats.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Exceptions;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;
using WSChat.Domain.Enums;

public record AddUserToChatCommand(long ChatId, long UserId) : IRequest<bool>;

public class AddUserToChatCommandHandler(
    IChatDbContext context,
    IMapper mapper) : IRequestHandler<AddUserToChatCommand, bool>
{
    public async Task<bool> Handle(AddUserToChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await context.Chats
            .Include(ch => ch.ChatUsers)
            .ThenInclude(cu => cu.User)
            .FirstOrDefaultAsync(cu => cu.Id == request.ChatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Chat), nameof(Chat.Id), request.ChatId);

        var chatUser = chat.ChatUsers.FirstOrDefault(cu => cu.UserId == request.UserId);

        if (chatUser is not null)
            throw new AlreadyExistsException(nameof(User), nameof(User.Id), request.UserId);

        if (chat.ChatUsers.Count >= 2)
            chat.ChatType = ChatType.Group;

        var newChatUser = mapper.Map<ChatUser>(request);
        await context.ChatUsers.AddAsync(newChatUser, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
