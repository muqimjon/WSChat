namespace WSChat.Application.Features.Chats.Models;

public class ChatDetailsResponse
{
    public long ChatId { get; set; }
    public string ChatName { get; set; } = string.Empty;
    public UserResponse Creator { get; set; } = default!;
    public ICollection<UserResponse> Members { get; set; } = [];
    public ICollection<MessageResponse> Messages { get; set; } = [];
}
