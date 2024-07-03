﻿using AutoMapper;
using Todo.Application.Task.Dto;
using Todo.Application.Team.Commands.UpdateTeamTask;
using Todo.Application.Users.Dto;
using Todo.Domain.Entities;

namespace Todo.Application.Team.Dto;

public class TeamProfile : Profile
{
    public TeamProfile()
    {
        CreateMap<TeamEntity, TeamDto>()
            .AfterMap((src, dst, context) => {
                dst.Tasks = src.Tasks.Count > 0 && src.Tasks != null ? context.Mapper.Map<ICollection<TodoEntity>, ICollection<TaskResumeDto>>(src.Tasks) : [];
                dst.Users = src.Users.Count > 0 && src.Users != null ? context.Mapper.Map<ICollection<UserEntity>, ICollection<UserResumeDto>>(src.Users) : [];
            });

        CreateMap<UpdateTaskByTeamCommand, TodoEntity>()
            .ForMember(src => src.Id, dst => dst.Ignore())
            .ForMember(src => src.TeamId, dst => dst.Ignore());
    }
}
