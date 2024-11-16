namespace WSChat.Application.Features.Users.Mappers;

using AutoMapper;
using WSChat.Application.Features.Users.Commands;
using WSChat.Application.Features.Users.Models;
using WSChat.Domain.Entities;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserProfileResponse>();
        CreateMap<UpdateUserProfileCommand, User>();
    }
}