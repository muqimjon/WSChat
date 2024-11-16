namespace WSChat.Application.Features.Messaging.Models;

using WSChat.Domain.Enums;

public class MessageResultDtoForProp
{
    public long Id { get; set; }
    public long? ReplyToMessageId { get; set; } = default;
    public string? Content { get; set; }
    public string? FilePath { get; set; }
    public MessageStatus Status { get; set; }
}