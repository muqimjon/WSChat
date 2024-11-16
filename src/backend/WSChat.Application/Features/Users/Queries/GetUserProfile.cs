namespace WSChat.Application.Features.Users.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Exceptions;
using WSChat.Application.Features.Chats.DTOs;
using WSChat.Application.Features.Users.DTOs;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;

public record GetUserProfileQuery(long UserId) : IRequest<UserResultDto>;

public class GetUserProfileQueryHandler(
    IChatDbContext context,
    IMapper mapper) :
    IRequestHandler<GetUserProfileQuery, UserResultDto>
{
    public async Task<UserResultDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.ChatUsers)
            .ThenInclude(cu => cu.Chat)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), nameof(User.Id), request.UserId);

        var result = mapper.Map<UserResultDto>(user);
        var chats = user.ChatUsers.Select(cu => cu.Chat);
        result.Chats = mapper.Map<ICollection<ChatResultDtoForProp>>(chats);

        return result;
    }
}
