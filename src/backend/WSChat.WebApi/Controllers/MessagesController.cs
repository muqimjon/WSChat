namespace WSChat.WebSocketApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using WSChat.Application.Features.Messaging.Commands;
using WSChat.WebSocketApi.Models;

public class MessagesController(IMediator mediator) : BaseController
{
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage(
        [FromForm] SendMessageCommand command,
        CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(command, cancellationToken)
        });
}