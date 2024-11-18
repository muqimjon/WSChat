namespace WSChat.WebSocketApi.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using WSChat.Application.Features.Messaging.Commands;
using WSChat.Application.Features.Messaging.Models;
using WSChat.WebSocketApi.Models;

public class MessagesController(IMediator mediator) : BaseController
{    /// <summary>
     /// Xabar yuborish
     /// </summary>
     /// <param name="command">Xabar yuborish uchun buyruq ma'lumotlari.</param>
     /// <param name="cancellationToken">Asinxronlikni boshqarish uchun tok.</param>
     /// <returns>Xabar yuborilishi haqida javob.</returns>
     /// <response code="200">Xabar muvaffaqiyatli yuborildi.</response>
     /// <response code="404">Yuboruvchi, ma'lumotlar bazasida mavjud emas yoki xabar yuborilmoqchi bo'lgan chat mavjud emas.</response>
     /// <response code="400">Foydalanuvchi chat ishtirokchisi emas yoki javob xabari noto'g'ri chatda joylashgan bo'lishi mumkin.</response>
    [HttpPost("send")]
    [ProducesResponseType(typeof(SuccessResponse<MessageResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendMessage(
        [FromForm] SendMessageCommand command,
        CancellationToken cancellationToken)
        => Ok(new Response
        {
            Data = await mediator.Send(command, cancellationToken)
        });

    /// <summary>
    /// Fayl yuklash
    /// </summary>
    /// <param name="cancellationToken">Asinxronlikni boshqarish uchun tok.</param>
    /// <param name="file">Yuklanayotgan fayl.</param>
    /// <returns>Yuklangan faylning URL manzili.</returns>
    /// <response code="200">Fayl muvaffaqiyatli yuklandi.</response>
    /// <response code="400">Fayl yuborilmagan yoki noto'g'ri yuborilgan.</response>
    /// <response code="500">Faylni yuklashda xatolik yuz berdi.</response>
    [HttpPost("file-upload")]
    [ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailResponse), StatusCodes.Status500InternalServerError)]
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
        return Ok(new Response
        {
            Data = fileUrl
        });
    }
}