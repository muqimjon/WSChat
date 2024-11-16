namespace WSChat.Application.Features.Chats.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Exceptions;
using WSChat.Application.Features.Chats.DTOs;
using WSChat.Application.Features.Users.DTOs;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;

public record GetChatByIdQuery(long ChatId) : IRequest<ChatResultDto>;

public class GetChatByIdQueryHandler(
    IChatDbContext context,
    IMapper mapper) : IRequestHandler<GetChatByIdQuery, ChatResultDto>
{
    public async Task<ChatResultDto> Handle(GetChatByIdQuery request, CancellationToken cancellationToken)
    {
        var chat = await context.Chats
            .Include(ch => ch.ChatUsers)
            .ThenInclude(cu => cu.User)
            .Include(ch => ch.Messages)
            .FirstOrDefaultAsync(ch => ch.Id == request.ChatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Chat), nameof(Chat.Id), request.ChatId);

        var chatResponse = mapper.Map<ChatResultDto>(chat);
        chatResponse.Users = mapper.Map<ICollection<UserResultDtoForProp>>(chat.ChatUsers.Select(cu => cu.User));

        return chatResponse;
    }
}
