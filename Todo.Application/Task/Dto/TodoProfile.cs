
using AutoMapper;
using Todo.Application.Task.Commands.CreateTask;
using Todo.Application.Task.Commands.UpdateTask;
using Todo.Application.Users.Dto;
using Todo.Domain.Entities;

namespace Todo.Application.Task.Dto;

public class TodoProfile : Profile
{

    public TodoProfile()
    {
        CreateMap<TodoEntity, TodoDto>().AfterMap((src, dst, context) => {
            dst.User = src.User != null
            ? context.Mapper.Map<UserEntity, UserDto>(src.User)
            : null;
        });
        CreateMap<CreateTaskCommand, TodoEntity>();

        CreateMap<UpdateTaskCommand, TodoEntity>();
    }
}
