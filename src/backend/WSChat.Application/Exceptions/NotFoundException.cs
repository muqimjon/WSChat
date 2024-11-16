using Microsoft.AspNetCore.Http;

namespace WSChat.Application.Exceptions;

public class NotFoundException(string model, string prop, dynamic value) :
    BaseException($"{model} is not found with {prop}: {value}", StatusCodes.Status404NotFound);
