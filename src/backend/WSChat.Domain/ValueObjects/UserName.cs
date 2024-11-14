namespace WSChat.Domain.ValueObjects;

public class UserName
{
    public string Value { get; }

    public UserName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Username cannot be empty.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;
}