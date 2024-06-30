using AutoMapper;
using Todo.Application.Users.Commands.Register;
using Todo.Domain.Entities;

namespace Todo.Application.Users.Dto;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserDto>();
        CreateMap<RegisterCommand, UserEntity>().ForMember(d => d.UserName, opt => opt.MapFrom(src => src.Email));
    }
}
