namespace WSChat.WebSocketApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using WSChat.Application.Features.Chats.Commands;
using WSChat.Application.Features.Chats.DTOs;
using WSChat.Application.Features.Chats.Queries;
using WSChat.WebSocketApi.Models;

public class ChatsController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Chat yaratish
    /// </summary>
    /// <param name="command">Chat yaratish uchun buyruq ma'lumotlari.</param>
    /// <param name="cancellationToken">Asinxronlikni boshqarish uchun tok.</param>
    /// <returns>Yaratilgan chat ma'lumotlari.</returns>
    /// <response code="200">Chat muvaffaqiyatli yaratildi.</response>
    /// <response code="404">Foydalanuvchi ma'lumotlar bazasidan topilmadi</response>
    [HttpPost("create")]
    [ProducesResponseType(typeof(SuccessResponse<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Creage(CreateChatCommand command, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(command, cancellationToken)
        });

    /// <summary>
    /// Chatga a'zo qo'shish
    /// </summary>
    /// <param name="command">Chatga foydalanuvchi qo'shish uchun buyruq ma'lumotlari.</param>
    /// <param name="cancellationToken">Asinxronlikni boshqarish uchun tok.</param>
    /// <returns>Chatga a'zo qo'shilganligi haqida javob.</returns>
    /// <response code="200">A'zo muvaffaqiyatli qo'shildi.</response>
    /// <response code="404">Foydalanuvchi yoki chat ma'lumotlar bazasida mavjud emas</response>
    /// <response code="409">Foydalanuvchi ushbu chatda mavjud</response>
    [HttpPut("add-user")]
    [ProducesResponseType(typeof(SuccessResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddUser(AddUserToChatCommand command, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(command, cancellationToken)
        });

    /// <summary>
    /// Foydalanuvchini chatdan chiqarish
    /// </summary>
    /// <param name="command">Foydalanuvchini chatdan chiqarish uchun buyruq ma'lumotlari.</param>
    /// <param name="cancellationToken">Asinxronlikni boshqarish uchun tok.</param>
    /// <returns>Foydalanuvchi muvaffaqiyatli chiqarilganligi haqida javob.</returns>
    /// <response code="200">Foydalanuvchi muvaffaqiyatli chatdan chiqarildi.</response>
    /// <response code="404">chat yoki foydalanuvchi ma'lumotlar bazasida mavjud emas</response>
    /// <response code="400">Foydalanuvchi ushbu chatda mavjud emas</response>
    [HttpDelete("remove-user")]
    [ProducesResponseType(typeof(SuccessResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveUser(RemoveUserFromChatCommand command, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(command, cancellationToken)
        });

    /// <summary>
    /// Chatni o'chirish
    /// </summary>
    /// <param name="command">Chatni o'chirish uchun buyruq ma'lumotlari.</param>
    /// <param name="cancellationToken">Asinxronlikni boshqarish uchun tok.</param>
    /// <returns>Chat muvaffaqiyatli o'chirilganligi haqida javob.</returns>
    /// <response code="200">Chat muvaffaqiyatli o'chirildi.</response>
    /// <response code="404">Id bo'yicha chat topilmadi</response>
    [HttpDelete("delete")]
    [ProducesResponseType(typeof(SuccessResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveChat(DeleteChatCommand command, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(command, cancellationToken)
        });

    /// <summary>
    /// Id bo'yicha chat tarixini olish
    /// </summary>
    /// <param name="chatId">Chatning identifikatori.</param>
    /// <param name="cancellationToken">Asinxronlikni boshqarish uchun tok.</param>
    /// <returns>Chatning tarixini olish uchun javob.</returns>
    /// <response code="200">Chat tarixini muvaffaqiyatli olish.</response>
    /// <response code="404">Chat ma'lumotlar bazasidan topilmadi</response>
    [HttpGet("get-by-id/{chatId:long}")]
    [ProducesResponseType(typeof(SuccessResponse<ChatResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long chatId, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(new GetChatByIdQuery(chatId), cancellationToken)
        });
}
