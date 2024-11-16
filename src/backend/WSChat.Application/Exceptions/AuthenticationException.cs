namespace WSChat.Application.Exceptions;

using Microsoft.AspNetCore.Http;

public class AuthenticationException(string message)
    : BaseException(message, StatusCodes.Status401Unauthorized);