using AutoMapper;
using Todo.Application.Users.Commands.Register;
using Todo.Domain.Entities;

namespace Todo.Application.Users.Mappers;

internal class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<RegisterCommand, UserEntity>().ForMember(d => d.UserName, opt => opt.MapFrom(src => src.Email));
    }
}
