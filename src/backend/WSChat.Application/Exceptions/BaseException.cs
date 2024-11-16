namespace WSChat.Application.Exceptions;

[Serializable]
public class BaseException(string message, int StatusCode) : Exception(message)
{
    public int StatusCode { get; set; } = StatusCode;

}