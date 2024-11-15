using WSChat.Domain.Enums;

namespace WSChat.Application.Features.Chats.Models;

public class MessageResponse
{
    public long Id { get; set; }
    public string? Content { get; set; }
    public string? FilePath { get; set; }
    public MessageStatus Status { get; set; }

    public MessageResponse? ReplyToMessage { get; set; } = default!;
}