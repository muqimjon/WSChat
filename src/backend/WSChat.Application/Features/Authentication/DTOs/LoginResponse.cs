using WSChat.Application.Features.Users.DTOs;

namespace WSChat.Application.Features.Authentication.DTOs;

public class LoginResponse
{
    public string? WebSocketUrl { get; set; }
    public string? Message { get; set; }
    public UserResultDto UserInfo { get; set; } = default!;
}
