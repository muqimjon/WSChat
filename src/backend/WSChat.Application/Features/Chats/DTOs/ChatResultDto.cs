namespace WSChat.Application.Features.Chats.DTOs;

using WSChat.Application.Features.Messaging.Models;
using WSChat.Application.Features.Users.DTOs;
using WSChat.Domain.Enums;

public class ChatResultDto
{
    public long Id { get; set; }
    public string ChatName { get; set; } = string.Empty;
    public ChatType ChatType { get; set; }

    public ICollection<MessageResultDtoForProp> Messages { get; set; } = default!;
    public ICollection<UserResultDtoForProp> Users { get; set; } = default!;
}
