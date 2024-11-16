namespace WSChat.Application.Exceptions;

public class CustomException(string message, int stausCode = 400) :
    BaseException(message, stausCode);
