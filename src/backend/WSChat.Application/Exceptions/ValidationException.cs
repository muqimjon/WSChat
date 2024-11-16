using Microsoft.AspNetCore.Http;

namespace WSChat.Application.Exceptions;

public class ValidationException(string message) :
    BaseException(message, StatusCodes.Status400BadRequest);
