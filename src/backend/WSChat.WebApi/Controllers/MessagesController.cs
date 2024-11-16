namespace WSChat.WebSocketApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using WSChat.Application.Features.Messaging.Commands;
using WSChat.Application.Features.Messaging.Queries;
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

    [HttpGet("get-by-chat-id/{chatId:long}")]
    public async Task<IActionResult> GetById(long chatId, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(new GetMessagesByChatIdQuery(chatId), cancellationToken)
        });
}