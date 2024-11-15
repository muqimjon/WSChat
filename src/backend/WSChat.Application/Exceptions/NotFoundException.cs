using Microsoft.AspNetCore.Http;

namespace WSChat.Application.Exceptions;

public class NotFoundException(string message) :
    BaseException(message, StatusCodes.Status404NotFound);
