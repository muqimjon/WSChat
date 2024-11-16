namespace WSChat.Application.Features.Chats.DTOs;

using WSChat.Domain.Enums;

public class ChatResultDtoForProp
{
    public long Id { get; set; }
    public string ChatName { get; set; } = string.Empty;
    public ChatType ChatType { get; set; }
}
