namespace WSChat.Application.Features.Users.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Exceptions;
using WSChat.Application.Features.Users.Models;
using WSChat.Application.Interfaces;

public record GetUserProfileQuery(long UserId) : IRequest<UserProfileResponse>;

public class GetUserProfileQueryHandler(
    IChatDbContext context,
    IMapper mapper) :
    IRequestHandler<GetUserProfileQuery, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.ChatUsers)
            .ThenInclude(cu => cu.Chat)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        return user is null ?
            throw new NotFoundException(nameof(user), nameof(user.Id), request.UserId) :
            mapper.Map<UserProfileResponse>(user);
    }
}
