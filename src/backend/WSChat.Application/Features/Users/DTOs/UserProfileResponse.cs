namespace WSChat.Application.Features.Users.Models;

using WSChat.Application.Features.Chats.Models;

public class UserProfileResponse
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<ChatResponse> ChatUsers { get; set; } = [];
}