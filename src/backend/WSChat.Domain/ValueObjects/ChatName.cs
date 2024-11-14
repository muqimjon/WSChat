namespace WSChat.Domain.ValueObjects;

public class ChatName
{
    public string Value { get; }

    public ChatName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Chat name cannot be empty.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;
}