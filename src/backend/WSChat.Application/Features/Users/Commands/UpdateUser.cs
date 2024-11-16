namespace WSChat.Application.Features.Users.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Exceptions;
using WSChat.Application.Features.Users.Models;
using WSChat.Application.Interfaces;

public record UpdateUserProfileCommand(long Id, string FirstName, string LastName, string Email, string Username) :
    IRequest<UserProfileResponse>;

public class UpdateUserProfileCommandHandler(
    IChatDbContext context,
    IMapper mapper) :
    IRequestHandler<UpdateUserProfileCommand, UserProfileResponse>
{
    public async Task<UserProfileResponse> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user is null)
            throw new NotFoundException(nameof(user), nameof(user.Id), request.Id);

        mapper.Map(request, user);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<UserProfileResponse>(user);
    }
}
