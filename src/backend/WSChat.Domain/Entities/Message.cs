namespace WSChat.Domain.Entities;

using System.Text.Json.Serialization;
using WSChat.Domain.Commons;
using WSChat.Domain.Enums;

public class Message : BaseEntity
{
    [JsonPropertyName("senderId")]
    public long SenderId { get; set; }

    [JsonPropertyName("chatId")]
    public long ChatId { get; set; }

    [JsonPropertyName("replyToMessageId")]
    public long? ReplyToMessageId { get; set; } = default;

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("filePath")]
    public string? FilePath { get; set; }
    public MessageStatus Status { get; set; }

    public User Sender { get; set; } = default!;
    public Chat Chat { get; set; } = default!;
    public Message? ReplyToMessage { get; set; } = default!;
}
