namespace WSChat.Application.Features.Users.Mappers;

using AutoMapper;
using WSChat.Application.Features.Authentication.Commands;
using WSChat.Application.Features.Users.Commands;
using WSChat.Application.Features.Users.DTOs;
using WSChat.Domain.Entities;

public class MessageMappingProfile : Profile
{
    public MessageMappingProfile()
    {
        CreateMap<User, UserResultDto>();
        CreateMap<User, UserResultDtoForProp>();
        CreateMap<UpdateUserProfileCommand, User>();
        CreateMap<RegisterUserCommand, User>();
    }
}