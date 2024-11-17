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

    [HttpPost("file-upload")]
    public async Task<IActionResult> Upload(CancellationToken cancellationToken, IFormFile file = default!)
    {
        if (file is null)
            return BadRequest(new
            {
                Message = "No file provided.",
                StatusCode = StatusCodes.Status400BadRequest
            });

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var fileDirectory = Path.Combine("wwwroot", "uploads");

        if (!Directory.Exists(fileDirectory))
            Directory.CreateDirectory(fileDirectory);

        var filePath = Path.Combine(fileDirectory, fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream, cancellationToken);
        }

        var fileUrl = Path.Combine("uploads", fileName).Replace("\\", "/");
        return Ok(new  Response
        {
            Data = fileUrl 
        });
    }

    [HttpGet("get-by-chat-id/{chatId:long}")]
    public async Task<IActionResult> GetById(long chatId, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(new GetMessagesByChatIdQuery(chatId), cancellationToken)
        });
}