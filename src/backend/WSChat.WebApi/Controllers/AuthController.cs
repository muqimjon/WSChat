namespace WSChat.WebSocketApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using WSChat.Application.Features.Authentication.Commands;
using WSChat.Application.Interfaces;
using WSChat.WebSocketApi.Models;

public class AuthController(
    IMediator mediator,
    IWebSocketService webSocketService) : BaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> LogIn(RegisterUserCommand command, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(command, cancellationToken)
        });

    [HttpPost("login")]
    public async Task<IActionResult> LogIn(LoginCommand command, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(command, cancellationToken)
        });

    [HttpGet("connect")]
    public async Task ConnectToChat(long userId)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var cancellationToken = HttpContext.RequestAborted;

            await webSocketService.HandleWebSocketAsync(userId, socket, cancellationToken);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }

}
