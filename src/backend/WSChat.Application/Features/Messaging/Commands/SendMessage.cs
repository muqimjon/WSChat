namespace WSChat.Application.Features.Messaging.Commands;

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using WSChat.Application.Exceptions;
using WSChat.Application.Features.Messaging.Models;
using WSChat.Application.Interfaces;
using WSChat.Domain.Entities;
using WSChat.Domain.Enums;

public record SendMessageCommand(
    long SenderId,
    long ChatId,
    string? Content,
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
        var user = await context.Users.FirstOrDefaultAsync(ch => ch.Id == request.SenderId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), nameof(User.Id), request.SenderId);

        var chat = await context.Chats.FirstOrDefaultAsync(ch => ch.Id == request.ChatId, cancellationToken)
            ?? throw new NotFoundException(nameof(Chat), nameof(Chat.Id), request.ChatId);

        var userChat = await context.ChatUsers
            .FirstOrDefaultAsync(cu => cu.UserId == request.SenderId && cu.ChatId == request.ChatId, cancellationToken)
            ?? throw new CustomException($"User with ID {request.SenderId} is not a member of the chat with ID {request.ChatId}.");

        var replyMessage = await GetReplyMessageAsync(request.ReplyToMessageId, cancellationToken);
        if (replyMessage is not null && replyMessage.ChatId != request.ChatId)
            throw new CustomException("Reply message is in another chat.");

        var message = mapper.Map<Message>(request);

        if (request.File is not null)
            message.FilePath = await UploadFile(request.File, cancellationToken);

        message.Status = MessageStatus.Sent;

        await context.Messages.AddAsync(message, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var messageDto = mapper.Map<MessageResultDto>(message);
        await webSocketService.SendMessageToChatMembersAsync(messageDto, cancellationToken);

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

    private static async Task<string> UploadFile(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null)
            return string.Empty;

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var fileDirectory = Path.Combine(Path.GetFullPath("wwwroot"), "uploads");

        if (!Directory.Exists(fileDirectory))
            Directory.CreateDirectory(fileDirectory);

        var filePath = Path.Combine(fileDirectory, fileName);

        using (FileStream fileStream = new(filePath, FileMode.OpenOrCreate))
        {
            await file.CopyToAsync(fileStream, cancellationToken);
        }

        var fileUrl = Path.Combine("uploads", fileName);

        return fileUrl;
    }
}
