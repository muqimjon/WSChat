namespace WSChat.Application.Features.Users.Commands;

using BCrypt.Net;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WSChat.Application.Features.Authentication.Models;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;

public record RegisterUserCommand(string Name, string Username, string Password) : IRequest<UserResponse>;

public class RegisterUserCommandHandler(IChatDbContext context) :
    IRequestHandler<RegisterUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = context.Users.FirstOrDefault(u => u.Username == request.Username);
        if (existingUser is not null)
            return new UserResponse
            {
                Success = false,
                Message = "Foydalanuvchi nomi band, iltimos boshqasini tanlang."
            };

        var hashedPassword = BCrypt.HashPassword(request.Password);

        var newUser = new User
        {
            Name = request.Name,
            Username = request.Username,
            PasswordHash = hashedPassword,
        };

        context.Users.Add(newUser);
        await context.SaveChangesAsync(cancellationToken);

        return new UserResponse
        {
            Success = true,
            Message = "Foydalanuvchi muvaffaqiyatli ro‘yxatdan o‘tdi. Id: " + newUser.Id,
        };
    }
}

