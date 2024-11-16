namespace WSChat.Application.Features.Messaging.Models;

using WSChat.Application.Features.Chats.DTOs;
using WSChat.Application.Features.Users.DTOs;
using WSChat.Domain.Enums;

public class MessageResultDto
{
    public long Id { get; set; }
    public string? Content { get; set; }
    public string? FilePath { get; set; }
    public MessageStatus Status { get; set; }

    public UserResultDtoForProp Sender { get; set; } = default!;
    public ChatResultDtoForProp Chat { get; set; } = default!;
    public MessageResultDtoForProp? ReplyToMessage { get; set; } = default!;
}
