namespace WSChat.Application.Features.Chats.DTOs;

using WSChat.Application.Features.Messaging.Models;
using WSChat.Domain.Enums;

public class ChatResultDtoForProp
{
    public long Id { get; set; }
    public string ChatName { get; set; } = string.Empty;
    public ChatType ChatType { get; set; }
    public ICollection<MessageResultDtoForProp> Messages { get; set; } = default!;
}
