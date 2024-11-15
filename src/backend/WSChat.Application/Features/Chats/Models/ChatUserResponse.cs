namespace WSChat.Application.Features.Chats.Models;

public class ChatUserResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public long ChatId { get; set; }
    public long UserId { get; set; }
    public string ChatName { get; internal set; } = string.Empty;
}