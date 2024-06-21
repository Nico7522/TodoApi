using AutoMapper;
using Todo.Application.Task.Dto;
using Todo.Application.Users.Dto;
using Todo.Domain.Entities;

namespace Todo.Application.Team.Dto;

public class TeamMapper : Profile
{
    public TeamMapper()
    {
        CreateMap<TeamEntity, TeamDto>()
            .AfterMap((src, dst, context) => {
                dst.Leader = src.Leader != null ? context.Mapper.Map<UserEntity, UserDto>(src.Leader) : null;
                dst.Tasks = src.Tasks.Count > 0 && src.Tasks != null ? context.Mapper.Map<ICollection<TodoEntity>, ICollection<TodoDto>>(src.Tasks) : [];
            });
    }
}
