namespace WSChat.Domain.Entities;

using WSChat.Domain.Commons;
using WSChat.Domain.Enums;

public class Chat : BaseEntity
{
    public string ChatName { get; set; } = string.Empty;
    public ChatType ChatType { get; set; }
    public ICollection<Message> Messages { get; set; } = default!;
    public ICollection<ChatUser> ChatUsers { get; set; } = default!;
}