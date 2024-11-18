namespace WSChat.WebSocketApi.Models;

public class SuccessResponse<T>
{
    public int StatusCode { get; set; } = StatusCodes.Status200OK;
    public string Message { get; set; } = "Success";
    public T Data { get; set; } = default!;
}

public class FailResponse
{
    public int StatusCode { get; set; } = StatusCodes.Status200OK;
    public string Message { get; set; } = "Success";
}