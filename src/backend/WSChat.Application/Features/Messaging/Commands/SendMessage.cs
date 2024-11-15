namespace WSChat.Application.Features.Messaging.Commands;

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WSChat.Application.Features.Messaging.Models;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;
using WSChat.Domain.Enums;

public record SendMessageCommand(
    long SenderId,
    long ChatId,
    string MessageContent,
    long? ReplyToMessageId,
    IFormFile? File) :
    IRequest<SendMessageResponse>;

public class SendMessageCommandHandler(
    IChatDbContext context,
    IWebSocketService webSocketService) :
    IRequestHandler<SendMessageCommand, SendMessageResponse>
{
    public async Task<SendMessageResponse> Handle([FromForm] SendMessageCommand request, CancellationToken cancellationToken)
    {
        var replyMessage = await GetReplyMessageAsync(request.ReplyToMessageId, cancellationToken);

        if (replyMessage is not null && replyMessage.ChatId != request.ChatId)
            return new SendMessageResponse
            {
                Success = false,
                Message = "Javob berilayotgan xabar boshqa chatga tegishli. Faqat bir xil chatdagi xabarga javob berishingiz mumkin."
            };

        string filePath = await SaveFileAsync(request.File, cancellationToken);

        var message = new Message
        {
            SenderId = request.SenderId,
            ChatId = request.ChatId,
            Content = request.MessageContent,
            Status = MessageStatus.Sent,
            ReplyToMessageId = request.ReplyToMessageId,
            FilePath = filePath,
            ReplyToMessage = replyMessage
        };

        await context.Messages.AddAsync(message, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await webSocketService.SendMessageToChatMembersAsync(message, cancellationToken);

        return new SendMessageResponse
        {
            Success = true,
            Message = "Xabar muvaffaqiyatli yuborildi"
        };
    }

    private async Task<Message?> GetReplyMessageAsync(long? replyToMessageId, CancellationToken cancellationToken)
    {
        if (!replyToMessageId.HasValue)
            return null;

        var replyMessage = await context.Messages
            .FirstOrDefaultAsync(m => m.Id == replyToMessageId.Value, cancellationToken);

        return replyMessage;
    }

    private static async Task<string> SaveFileAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null)
            return string.Empty;

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var fileDirectory = Path.Combine(Path.GetFullPath("wwwroot"), "uploads");

        if (!Directory.Exists(fileDirectory))
            Directory.CreateDirectory(fileDirectory);

        var filePath = Path.Combine(fileDirectory, fileName);

        using (FileStream fileStream = new(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream, cancellationToken);
        }

        var fileUrl = Path.Combine("uploads", fileName);

        return fileUrl;
    }
}
