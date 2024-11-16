namespace WSChat.Domain.Entities;

using WSChat.Domain.Commons;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<Message> Messages { get; set; } = [];
    public ICollection<ChatUser> ChatUsers { get; set; } = [];

    public string GetFullName()
        => string.Concat(FirstName, (char)32, LastName);
}