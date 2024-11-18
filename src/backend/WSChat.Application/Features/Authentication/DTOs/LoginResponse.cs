using WSChat.Application.Features.Users.DTOs;

namespace WSChat.Application.Features.Authentication.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string WebSocketUrl { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public UserResultDto UserInfo { get; set; } = default!;
}
