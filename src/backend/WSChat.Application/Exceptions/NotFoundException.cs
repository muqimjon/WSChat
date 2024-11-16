namespace WSChat.Application.Exceptions;

using Microsoft.AspNetCore.Http;

public class NotFoundException(string model, string prop, dynamic value) :
    BaseException($"{model} is not found with {prop}: {value}", StatusCodes.Status404NotFound);
