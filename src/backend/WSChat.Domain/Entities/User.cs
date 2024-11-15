namespace WSChat.Domain.Entities;

using WSChat.Domain.Commons;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<Message> Messages { get; set; } = [];
    public ICollection<ChatUser> ChatUsers { get; set; } = [];
}