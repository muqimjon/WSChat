namespace WSChat.Domain.Events;

public class UserJoinedChatEvent(long userId, long chatId)
{
    public long UserId { get; } = userId;
    public long ChatId { get; } = chatId;
}