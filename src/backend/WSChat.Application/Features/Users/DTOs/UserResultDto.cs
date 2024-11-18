using WSChat.Application.Features.Chats.DTOs;

namespace WSChat.Application.Features.Users.DTOs;

public class UserResultDto
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public IEnumerable<ChatResultDtoForProp> Chats { get; set; } = [];
}