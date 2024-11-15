namespace WSChat.Application.Features.Users.Queries;

using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Features.Users.Models;
using WSChat.Application.Interfaces;

public record GetUserProfileQuery(long UserId) : IRequest<UserProfileResponse>;

public class GetUserProfileQueryHandler(IChatDbContext context) : IRequestHandler<GetUserProfileQuery, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.ChatUsers)
            .ThenInclude(cu => cu.Chat)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            return new UserProfileResponse();

        var userResponse = new UserProfileResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Name = user.Name,
            Chats = user.ChatUsers.Select(cu => new ChatSummaryResponse
            {
                ChatId = cu.Chat.Id,
                ChatName = cu.Chat.ChatName
            }).ToList()
        };

        return userResponse;
    }
}
