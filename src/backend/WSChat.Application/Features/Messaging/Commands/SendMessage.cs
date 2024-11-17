namespace WSChat.Application.Features.Messaging.Commands;

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
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
    string Content,
    long? ReplyToMessageId,
    IFormFile? File) :
    IRequest<long>;

public class SendMessageCommandHandler(
    IChatDbContext context,
    IWebSocketService webSocketService,
    IMapper mapper,
    IMediator mediator) :
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

        var userChat = await context.ChatUsers
            .FirstOrDefaultAsync(cu => cu.UserId == request.SenderId && cu.ChatId == request.ChatId, cancellationToken)
            ?? throw new CustomException($"User with ID {request.SenderId} is not a member of the chat with ID {request.ChatId}.");

        var message = mapper.Map<Message>(request);
        
        if(request.File is not null)
            message.FilePath = await mediator.Send(new UploadFileCommand(request.File), cancellationToken);

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
}
