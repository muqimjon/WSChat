namespace WSChat.Application.Features.Users.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Exceptions;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;

public record DeleteUserCommand(long UserId, string Password) : IRequest<bool>;
public class DeleteUserCommandHandler(IChatDbContext context) : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), nameof(User.Id), request.UserId);

        var isCorrect = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (isCorrect)
            throw new AuthenticationException("Invalid password.");

        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}