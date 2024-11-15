namespace WSChat.Application.Features.Authentication.Commands;

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Features.Authentication.Models;
using WSChat.Application.Interfaces;

public class LoginCommand : IRequest<LoginResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandHandler(IChatDbContext context, IHttpContextAccessor accessor) : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (user is not null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            var host = accessor.HttpContext?.Request.Host.Value ?? "localhost";
            var webSocketUrl = $"ws://{host}/api/Auth/connect?userId={user.Id}";

            return new LoginResponse
            {
                WebSocketUrl = webSocketUrl,
                Message = "Login successful"
            };
        }

        return new LoginResponse
        {
            Message = "Invalid username or password."
        };
    }
}
