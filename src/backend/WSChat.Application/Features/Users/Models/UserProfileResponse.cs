namespace WSChat.Application.Features.Users.Models;

public class UserProfileResponse
{
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public ICollection<ChatSummaryResponse> Chats { get; set; } = [];
}
