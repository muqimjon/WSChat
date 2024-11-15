using MediatR;
using Microsoft.AspNetCore.Mvc;
using WSChat.Application.Features.Messaging.Commands;

namespace WSChat.WebSocketApi.Controllers;

public class MessagesController(IMediator mediator) : BaseController
{
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromForm] SendMessageCommand command, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);

        if (response.Success)
            return Ok(response);
        else
            return BadRequest(response);
    }
}