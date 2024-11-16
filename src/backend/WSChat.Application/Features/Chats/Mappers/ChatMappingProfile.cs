namespace WSChat.Application.Features.Users.Mappers;

using AutoMapper;
using WSChat.Application.Features.Chats.Commands;
using WSChat.Application.Features.Chats.DTOs;
using WSChat.Domain.Entities;

public class ChatMappingProfile : Profile
{
    public ChatMappingProfile()
    {
        CreateMap<Chat, ChatResultDto>();
        CreateMap<Chat, ChatResultDtoForProp>();
        CreateMap<CreateChatCommand, Chat>();
        CreateMap<AddUserToChatCommand, ChatUser>();
    }
}