namespace WSChat.Application.Features.Messages.Mappers;

using AutoMapper;
using WSChat.Application.Features.Messaging.Commands;
using WSChat.Application.Features.Messaging.Models;
using WSChat.Domain.Entities;

public class MessageMappingProfile : Profile
{
    public MessageMappingProfile()
    {
        CreateMap<Message, MessageResultDto>();
        CreateMap<Message, MessageResultDtoForProp>();
        CreateMap<SendMessageCommand, Message>();
    }
}