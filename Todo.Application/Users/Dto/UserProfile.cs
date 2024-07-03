using AutoMapper;
using System.Security.Cryptography;
using Todo.Application.Task.Dto;
using Todo.Application.Users.Commands.Register;
using Todo.Domain.Entities;

namespace Todo.Application.Users.Dto;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserDto>()

        .AfterMap((src, dst, context) =>
        {
            dst.Tasks = src.Tasks.Count > 0 && src.Tasks != null ? context.Mapper.Map<ICollection<TodoEntity>, ICollection<TaskResumeDto>>(src.Tasks) : [];
        });
        CreateMap<RegisterCommand, UserEntity>().ForMember(d => d.UserName, opt => opt.MapFrom(src => src.Email));

        CreateMap<UserEntity, UserResumeDto>();

    }
}
