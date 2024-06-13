
using AutoMapper;
using Todo.Domain.Entities;

namespace Todo.Application.Task.Dto;

public class TodoMapper : Profile
{

    public TodoMapper()
    {
        CreateMap<TodoEntity, TodoDto>();
    }
}
