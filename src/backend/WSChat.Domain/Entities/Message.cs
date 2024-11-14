namespace WSChat.Domain.Entities;

using WSChat.Domain.Commons;
using WSChat.Domain.Enums;

public class Message : BaseEntity
{
    public long SenderId { get; set; }
    public long ChatId { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageStatus Status { get; set; } = MessageStatus.Sent;
    public DateTime SentDate { get; set; } = DateTime.UtcNow;

    public User Sender { get; set; } = default!;
    public Chat Chat { get; set; } = default!;
}