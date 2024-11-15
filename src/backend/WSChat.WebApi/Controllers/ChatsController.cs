namespace WSChat.WebSocketApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using WSChat.Application.Features.Chats.Commands;
using WSChat.Application.Features.Chats.Queries;

public class ChatsController(IMediator mediator) : BaseController
{
    [HttpPost("create")]
    public async Task<IActionResult> Creage(CreateChatCommand command, CancellationToken cancellationToken)
        => Ok(await mediator.Send(command, cancellationToken));

    [HttpPut("add-user")]
    public async Task<IActionResult> AddUser(AddUserToChatCommand command, CancellationToken cancellationToken)
        => Ok(await mediator.Send(command, cancellationToken));

    [HttpDelete("remove-user")]
    public async Task<IActionResult> RemoveUser(RemoveUserFromChatCommand command, CancellationToken cancellationToken)
        => Ok(await mediator.Send(command, cancellationToken));

    [HttpDelete("delete")]
    public async Task<IActionResult> RemoveChat(DeleteChatCommand command, CancellationToken cancellationToken)
        => Ok(await mediator.Send(command, cancellationToken));

    [HttpGet("get-by-id/{Id:long}")]
    public async Task<IActionResult> GetById(long Id, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetChatByIdQuery(Id), cancellationToken));
}
