namespace WSChat.WebSocketApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using WSChat.Application.Features.Users.Commands;
using WSChat.Application.Features.Users.Queries;
using WSChat.WebSocketApi.Models;

public class UsersController(IMediator mediator) : BaseController
{
    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateUserProfileCommand command, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(command, cancellationToken)
        });

    [HttpGet("get-by-id/{Id:long}")]
    public async Task<IActionResult> GetById(long Id, CancellationToken cancellationToken)
    => Ok(new Response
    {
        Data = await mediator.Send(new GetUserProfileQuery(Id), cancellationToken)
    });

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteById(DeleteUserCommand command, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(command, cancellationToken)
        });
}
