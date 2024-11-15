namespace WSChat.Application.Features.Users.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Features.Authentication.Models;
using WSChat.Application.Interfaces;

public record DeleteUserCommand(long UserId, string Password) : IRequest<UserResponse>;
public class DeleteUserCommandHandler(IChatDbContext context) : IRequestHandler<DeleteUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            return new UserResponse
            {
                Success = false,
                Message = "Foydalanuvchi topilmadi"
            };

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
            return new UserResponse
            {
                Success = false,
                Message = "Parol noto'g'ri"
            };

        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);

        return new UserResponse
        {
            Success = true,
            Message = "Foydalanuvchi muvaffaqiyatli o'chirildi"
        };
    }
}