namespace WSChat.Application.Features.Messaging.Queries;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WSChat.Application.Features.Messaging.Models;
using WSChat.Application.Interfaces;

public record GetMessagesByChatIdQuery(long ChatId) : IRequest<IEnumerable<MessageResultDto>>;

public class GetMessagesByChatIdQueryHandler(
    IChatDbContext context,
    IMapper mapper) :
    IRequestHandler<GetMessagesByChatIdQuery, IEnumerable<MessageResultDto>>
{
    public async Task<IEnumerable<MessageResultDto>> Handle(GetMessagesByChatIdQuery request, CancellationToken cancellationToken)
    {
        var messages = await context.Messages
            .Include(u => u.Chat)
            .Include(m => m.Sender)
            .Where(u => u.Id == request.ChatId)
            .ToListAsync(cancellationToken);

        return mapper.Map<IEnumerable<MessageResultDto>>(messages);
    }
}
