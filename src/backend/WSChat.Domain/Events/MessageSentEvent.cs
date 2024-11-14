namespace WSChat.Domain.Events;

public class MessageSentEvent(long messageId, long chatId, long senderId)
{
    public long MessageId { get; } = messageId;
    public long ChatId { get; } = chatId;
    public long SenderId { get; } = senderId;
}
