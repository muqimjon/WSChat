namespace WSChat.Application.Features.Users.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Interfaces;

public record UpdateUserProfileCommand(long UserId, string Name, string Email, string Username) :
    IRequest<Models.UserResponse>;

public class UpdateUserProfileCommandHandler(IChatDbContext context) :
    IRequestHandler<UpdateUserProfileCommand, Models.UserResponse>
{
    public async Task<Models.UserResponse> Handle(
        UpdateUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
            return new Models.UserResponse
            {
                Success = false,
                Message = "Foydalanuvchi topilmadi"
            };

        user.Name = request.Name;
        user.Username = request.Username;
        await context.SaveChangesAsync(cancellationToken);

        return new Models.UserResponse
        {
            Success = true,
            Message = "Foydalanuvchi profili muvaffaqiyatli yangilandi"
        };
    }
}
