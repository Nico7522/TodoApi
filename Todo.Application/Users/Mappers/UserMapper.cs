using AutoMapper;
using Todo.Application.Users.Commands.Register;
using Todo.Application.Users.Dto;
using Todo.Domain.Entities;

namespace Todo.Application.Users.Mappers;

internal class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserEntity, UserDto>();
        CreateMap<RegisterCommand, UserEntity>().ForMember(d => d.UserName, opt => opt.MapFrom(src => src.Email));
    }
}
