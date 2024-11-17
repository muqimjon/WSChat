namespace WSChat.WebSocketApi.Models;

public class Message
{
    public long SenderId { get; set; }
    public long ChatId { get; set; }
    public long? ReplyToMessageId { get; set; } = default;
    public string? Content { get; set; }
    public string? FilePath { get; set; }
}
