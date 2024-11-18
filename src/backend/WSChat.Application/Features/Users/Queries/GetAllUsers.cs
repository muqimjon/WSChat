namespace WSChat.Application.Features.Users.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Features.Chats.DTOs;
using WSChat.Application.Features.Users.DTOs;
using WSChat.Application.Interfaces;

public record GetAllUsersQuery() : IRequest<IEnumerable<UserResultDto>>;

public class GetAllUsersQueryHandler(
    IChatDbContext context,
    IMapper mapper) :
    IRequestHandler<GetAllUsersQuery, IEnumerable<UserResultDto>>
{
    public async Task<IEnumerable<UserResultDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await context.Users
            .Include(u => u.ChatUsers)
            .ThenInclude(u => u.Chat)
            .AsNoTracking().ToListAsync(cancellationToken);
        var userDtos = mapper.Map<IEnumerable<UserResultDto>>(users);

        foreach (var userDto in userDtos)
        {
            var chats = users.First(u => u.Id == userDto.Id).ChatUsers.Select(cu => cu.Chat);
            userDto.Chats = mapper.Map<ICollection<ChatResultDtoForProp>>(chats);
        }

        return userDtos;
    }
}
