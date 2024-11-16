namespace WSChat.Application.Exceptions;

using Microsoft.AspNetCore.Http;

internal class AlreadyExistsException(string model, string prop, dynamic value) :
    BaseException($"{model} is already exist with {prop}: {value}", StatusCodes.Status409Conflict);