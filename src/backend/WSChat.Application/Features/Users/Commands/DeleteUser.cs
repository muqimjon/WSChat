﻿namespace WSChat.Application.Features.Users.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Exceptions;
using WSChat.Application.Interfaces;

public record DeleteUserCommand(long UserId, string Password) : IRequest<bool>;
public class DeleteUserCommandHandler(IChatDbContext context) : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            throw new NotFoundException(nameof(user), nameof(user.Id), request.UserId);

        _ = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash) ?
            throw new AuthenticationException("Invalid password.") : (bool)default;

        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}