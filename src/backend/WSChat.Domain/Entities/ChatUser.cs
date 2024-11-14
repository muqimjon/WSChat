namespace WSChat.Domain.Entities;

public class ChatUser
{
    public long ChatId { get; set; }
    public long UserId { get; set; }

    public Chat Chat { get; set; } = default!;
    public User User { get; set; } = default!;
}