namespace WSChat.WebSocketApi.Middlewares;

using WSChat.Application.Exceptions;

public class ExceptionHandlerMiddleware(RequestDelegate next,
    ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BaseException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            logger.LogError($"Custom exception caught: {ex.Message}");

            await context.Response.WriteAsJsonAsync(new
            {
                context.Response.StatusCode,
                ex.Message,
            });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            logger.LogError($"Unhandled exception caught: {ex}");

            await context.Response.WriteAsJsonAsync(new
            {
                context.Response.StatusCode,
                Message = "An unexpected error occurred.",
            });
        }
    }
}
