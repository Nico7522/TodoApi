
using AutoMapper;
using Todo.Application.Task.Commands.CreateTask;
using Todo.Application.Task.Commands.UpdateTask;
using Todo.Domain.Entities;

namespace Todo.Application.Task.Dto;

public class TodoMapper : Profile
{

    public TodoMapper()
    {
        CreateMap<TodoEntity, TodoDto>();
        CreateMap<CreateTaskCommand, TodoEntity>();

        CreateMap<UpdateTaskCommand, TodoEntity>();
    }
}
