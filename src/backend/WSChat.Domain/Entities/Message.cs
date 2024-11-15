namespace WSChat.Domain.Entities;

using System.Text.Json.Serialization;
using WSChat.Domain.Commons;
using WSChat.Domain.Enums;

public class Message : BaseEntity
{
    [JsonPropertyName("senderId")]
    public long SenderId { get; set; }

    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    [JsonPropertyName("reply_to_message_id")]
    public long? ReplyToMessageId { get; set; } = default;

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("file_path")]
    public string? FilePath { get; set; }

    [JsonPropertyName("status")]
    public MessageStatus Status { get; set; } = MessageStatus.Sent;

    public User Sender { get; set; } = default!;
    public Chat Chat { get; set; } = default!;
    public Message? ReplyToMessage { get; set; } = default!;
}
