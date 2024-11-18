namespace WSChat.WebSocketApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WSChat.Application.Features.Authentication.Commands;
using WSChat.Application.Features.Authentication.DTOs;
using WSChat.Application.Interfaces;
using WSChat.WebSocketApi.Models;

public class AuthController(
    IMediator mediator,
    IWebSocketService webSocketService) : BaseController
{
    /// <summary>
    /// Ro'yxatdan o'tish
    /// </summary>
    /// <param name="command">Ro'yxatdan o'tish uchun foydalanuvchi ma'lumotlari.</param>
    /// <param name="cancellationToken">Asinxronlikni boshqarish uchun tok.</param>
    /// <returns>Foydalanuvchi muvaffaqiyatli ro'yxatdan o'tganligi haqida javob.</returns>
    /// <response code="200">Foydalanuvchi muvaffaqiyatli ro'yxatdan o'tdi.</response>
    /// <response code="409">Foydalanuvchi ma'lumotlar bazasida mavjud</response>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(SuccessResponse<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> LogIn(RegisterUserCommand command, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(command, cancellationToken)
        });

    /// <summary>
    /// Tizimga kirish
    /// </summary>
    /// <param name="command">Tizimga kirish uchun foydalanuvchi ma'lumotlari.</param>
    /// <param name="cancellationToken">Asinxronlikni boshqarish uchun tok.</param>
    /// <returns>Tizimga kirish uchun token.</returns>
    /// <response code="200">Foydalanuvchi tizimga muvaffaqiyatli kirdi.</response>
    /// <response code="401">Login yoki parol noto'g'ri</response>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(SuccessResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogIn(LoginCommand command, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(command, cancellationToken)
        });

    /// <summary>
    /// Websocket orqali ulash uchun endpoint. Swagger orqali foydalanishni imkoni yo'q!!!
    /// </summary>
    /// <param name="userId">Foydalanuvchi identifikatori.</param>
    /// <returns>Inline error</returns>
    /// <response code="200">Xabar almashish uchun obyekt tuzilishi</response>
    [AllowAnonymous]
    [HttpGet("connect")]
    [ProducesResponseType(typeof(Message), StatusCodes.Status200OK)]
    public async Task ConnectToChat(long userId)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var cancellationToken = HttpContext.RequestAborted;

            await webSocketService.HandleWebSocketAsync(userId, socket, cancellationToken);
        }
        else
            HttpContext.Response.StatusCode = 400;
    }
}
