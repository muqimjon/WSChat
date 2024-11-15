namespace WSChat.Domain.Entities;

using WSChat.Domain.Commons;
using WSChat.Domain.Enums;

public class Message : BaseEntity
{
    public long SenderId { get; set; }
    public long ChatId { get; set; }
    public long? ReplyToMessageId { get; set; } = default;
    public string? Content { get; set; }
    public string? FilePath { get; set; }
    public MessageStatus Status { get; set; } = MessageStatus.Sent;

    public User Sender { get; set; } = default!;
    public Chat Chat { get; set; } = default!;
    public Message? ReplyToMessage { get; set; } = default!;
}
