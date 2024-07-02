using AutoMapper;
using Todo.Application.Task.Commands.CreateTask;
using Todo.Application.Task.Commands.UpdateTask;
using Todo.Domain.Entities;

namespace Todo.Application.Task.Dto;

public class TaskProfile : Profile
{

    public TaskProfile()
    {
        CreateMap<TodoEntity, TaskDto>();
        CreateMap<TodoEntity, TaskForUserDto>();
        CreateMap<CreateTaskCommand, TodoEntity>();
        CreateMap<UpdateTaskCommand, TodoEntity>();
    }
}
