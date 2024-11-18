namespace WSChat.Application.Features.Authentication.Commands;

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WSChat.Application.Exceptions;
using WSChat.Application.Features.Authentication.DTOs;
using WSChat.Application.Features.Chats.DTOs;
using WSChat.Application.Features.Users.DTOs;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;

public class LoginCommand : IRequest<LoginResponse>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandHandler(
    IChatDbContext context,
    IHttpContextAccessor accessor,
    IMapper mapper,
    IConfiguration configuration) :
    IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.ChatUsers)
                .ThenInclude(cu => cu.Chat)
                    .ThenInclude(c => c.Messages)
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new AuthenticationException("Invalid username or password.");

        var host = accessor.HttpContext?.Request.Host.Value ?? "localhost";
        var webSocketUrl = $"ws://{host}/api/auth/connect?userId={user.Id}";

        var response = new LoginResponse
        {
            Message = "Login successful",
            WebSocketUrl = webSocketUrl,
            UserInfo = mapper.Map<UserResultDto>(user),
            Token = GenerateToken(user),
        };

        var chats = user.ChatUsers.Select(cu => cu.Chat);
        response.UserInfo.Chats = mapper.Map<IEnumerable<ChatResultDtoForProp>>(chats);

        return response;
    }

    private string GenerateToken(User user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(nameof(user.Id), user.Id.ToString()),
                new Claim(nameof(user.Username), user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
            }),

            Expires = DateTime.UtcNow.AddHours(5),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!)),
                SecurityAlgorithms.HmacSha256Signature),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
