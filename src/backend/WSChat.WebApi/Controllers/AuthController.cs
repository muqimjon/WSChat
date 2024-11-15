using MediatR;
using Microsoft.AspNetCore.Mvc;
using WSChat.Application.Features.Authentication.Commands;
using WSChat.Application.Features.Users.Commands;

namespace WSChat.WebSocketApi.Controllers;

public class AuthController(IMediator mediator) : BaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> LogIn(RegisterUserCommand command, CancellationToken cancellationToken)
        => Ok(await mediator.Send(command, cancellationToken));

    [HttpPost("login")]
    public async Task<IActionResult> LogIn(LoginCommand command, CancellationToken cancellationToken)
        => Ok(await mediator.Send(command, cancellationToken));
}
