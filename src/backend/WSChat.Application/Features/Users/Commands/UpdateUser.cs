namespace WSChat.Application.Features.Users.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Exceptions;
using WSChat.Application.Features.Users.DTOs;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;

public record UpdateUserProfileCommand(long Id, string FirstName, string LastName, string Email, string Username) :
    IRequest<UserResultDto>;

public class UpdateUserProfileCommandHandler(
    IChatDbContext context,
    IMapper mapper) :
    IRequestHandler<UpdateUserProfileCommand, UserResultDto>
{
    public async Task<UserResultDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(User), nameof(User.Id), request.Id);

        mapper.Map(request, user);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<UserResultDto>(user);
    }
}
