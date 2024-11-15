namespace WSChat.Application.Features.Messaging.Models;

public class SendMessageResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
