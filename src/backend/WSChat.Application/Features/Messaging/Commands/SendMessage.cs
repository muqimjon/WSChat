namespace WSChat.Application.Features.Messaging.Commands;

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WSChat.Application.Exceptions;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;
using WSChat.Domain.Enums;

public record SendMessageCommand(
    long SenderId,
    long ChatId,
    string MessageContent,
    long? ReplyToMessageId,
    IFormFile? File) :
    IRequest<long>;

public class SendMessageCommandHandler(
    IChatDbContext context,
    IWebSocketService webSocketService,
    IMapper mapper) :
    IRequestHandler<SendMessageCommand, long>
{
    public async Task<long> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var chat = await context.Chats.FirstOrDefaultAsync(ch => ch.Id == request.ChatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Chat), nameof(Chat.Id), request.ChatId);

        var replyMessage = await GetReplyMessageAsync(request.ReplyToMessageId, cancellationToken);
        if (replyMessage is not null && replyMessage.ChatId != request.ChatId)
            throw new CustomException("Reply message is in another chat.");

        var user = await context.Users.FirstOrDefaultAsync(ch => ch.Id == request.SenderId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), nameof(User.Id), request.SenderId);

        var message = mapper.Map<Message>(request);
        message.FilePath = await SaveFileAsync(request.File, cancellationToken);
        message.Status = MessageStatus.Sent;
        message.Chat = chat;
        message.Sender = user;
        message.ReplyToMessage = replyMessage;

        await context.Messages.AddAsync(message, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await webSocketService.SendMessageToChatMembersAsync(message, cancellationToken);

        return message.Id;
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
