namespace WSChat.WebSocketApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using WSChat.Application.Features.Users.Commands;
using WSChat.Application.Features.Users.DTOs;
using WSChat.Application.Features.Users.Queries;
using WSChat.WebSocketApi.Models;

public class UsersController(IMediator mediator) : BaseController
{
    /// <summary>
    /// Foydalanuvchi ma'lumotlarini yangilash
    /// </summary>
    /// <param name="command">Yangilash uchun foydalanuvchi profili ma'lumotlari.</param>
    /// <param name="cancellationToken">Asinxronlikni boshqarish uchun tok.</param>
    /// <returns>Yangilangan foydalanuvchi ma'lumotlari.</returns>
    /// <response code="200">Ma'lumot muvaffaqiyatli yangilandi.</response>
    /// <response code="404">Buyruqda xatoliklar bor.</response>
    [HttpPut("update")]
    [ProducesResponseType(typeof(SuccessResponse<UserResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(UpdateUserProfileCommand command, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(command, cancellationToken)
        });

    /// <summary>
    /// Foydalanuvchi hisobini o'chirish
    /// </summary>
    /// <param name="command">Foydalanuvchini o'chirish uchun buyruq ma'lumotlari.</param>
    /// <param name="cancellationToken">Asinxronlikni boshqarish uchun tok.</param>
    /// <returns>Foydalanuvchi hisobini muvaffaqiyatli o'chirish.</returns>
    /// <response code="200">Hisob muvaffaqiyatli o'chirildi.</response>
    /// <response code="404">Foydalanuvchi topilmadi.</response>
    /// <response code="401">Noto'g'ri parol kiritildi.</response>
    [HttpDelete("delete")]
    [ProducesResponseType(typeof(SuccessResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteById(DeleteUserCommand command, CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(command, cancellationToken)
        });

    /// <summary>
    /// Id bo'yicha foydalanuvchi ma'lumotlarini olish
    /// </summary>
    /// <param name="userId">Foydalanuvchi identifikatori (ID).</param>
    /// <param name="cancellationToken">Asinxronlikni boshqarish uchun tok.</param>
    /// <returns>Berilgan userId bo'yicha foydalanuvchi ma'lumotlari.</returns>
    /// <response code="200">Foydalanuvchi ma'lumotlari muvaffaqiyatli olindi.</response>
    /// <response code="404">Foydalanuvchi topilmadi.</response>
    [HttpGet("get-by-id/{userId:long}")]
    [ProducesResponseType(typeof(SuccessResponse<UserResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long userId, CancellationToken cancellationToken)
    => Ok(new Response
    {
        Data = await mediator.Send(new GetUserProfileQuery(userId), cancellationToken)
    });

    /// <summary>
    /// Barcha foydalanuvchilar ma'lumotini olish
    /// </summary>
    /// <param name="cancellationToken">Asinxronlikni boshqarish uchun tok.</param>
    /// <returns>Barcha foydalanuvchilar ro'yxatini olish.</returns>
    /// <response code="200">Barcha foydalanuvchilar muvaffaqiyatli olindi.</response>
    [HttpGet("get-all")]
    [ProducesResponseType(typeof(SuccessResponse<IEnumerable<UserResultDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(new GetAllUsersQuery(), cancellationToken)
        });
}
