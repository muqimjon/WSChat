namespace WSChat.Application.Features.Authentication.Commands;

using AutoMapper;
using BCrypt.Net;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WSChat.Application.Exceptions;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string Password) : IRequest<long>;

public class RegisterUserCommandHandler(
    IChatDbContext context,
    IMapper mapper) :
    IRequestHandler<RegisterUserCommand, long>
{
    public async Task<long> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = context.Users.FirstOrDefault(u => u.Username == request.Username);
        if (existingUser is not null)
            throw new AlreadyExistsException(nameof(User), nameof(User.Username), request.Username);

        var newUser = mapper.Map<User>(request);
        newUser.PasswordHash = BCrypt.HashPassword(request.Password);
        await context.Users.AddAsync(newUser, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return newUser.Id;
    }
}

