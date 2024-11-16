namespace WSChat.WebSocketApi.Models;

public class Response
{
    public int StatusCode { get; set; } = StatusCodes.Status200OK;
    public string Message { get; set; } = "Success";
    public object Data { get; set; } = default!;
}
