namespace WSChat.Application.Features.Authentication.Commands;

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using WSChat.Application.Features.Authentication.DTOs;
using WSChat.Application.Features.Chats.DTOs;
using WSChat.Application.Features.Users.DTOs;
using WSChat.Application.Interfaces;

public class LoginCommand : IRequest<LoginResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandHandler(
    IChatDbContext context,
    IHttpContextAccessor accessor,
    IMapper mapper) : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.ChatUsers)
            .ThenInclude(cu => cu.Chat)
            .ThenInclude(c => c.Messages)
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (user is not null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            var host = accessor.HttpContext?.Request.Host.Value ?? "localhost";
            var webSocketUrl = $"ws://{host}/api/Auth/connect?userId={user.Id}";

            var response = new LoginResponse
            {
                WebSocketUrl = webSocketUrl,
                Message = "Login successful",
                UserInfo = mapper.Map<UserResultDto>(user)
            };

            var chats = user.ChatUsers.Select(cu => cu.Chat);
            response.UserInfo.Chats = mapper.Map<ICollection<ChatResultDtoForProp>>(chats);

            return response;
        }

        throw new AuthenticationException("Invalid username or password.");
    }
}
