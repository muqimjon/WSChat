namespace WSChat.Application.Common;

using Microsoft.AspNetCore.Http;

public class HttpContextHelper
{
#nullable disable
    public static IHttpContextAccessor Accessor { get; set; }

    private static HttpContext HttpContext => Accessor?.HttpContext;

    public static IHeaderDictionary ResponseHeaders => HttpContext?.Response?.Headers;

    public static long? GetUserId
    {
        get
        {
            var userId = HttpContext?.User?.FindFirst("Id")?.Value;
            return long.TryParse(userId, out var parsedUserId) ? parsedUserId : null;
        }
    }
    //public static string UserRole => HttpContext?.User?.FindFirst("Role")?.Value;
}