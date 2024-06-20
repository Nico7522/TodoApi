using MediatR;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Application.Task.Commands.UpdateTask;

public class UpdateTaskCommand : IRequest
{
    public Guid Id { get; init; } 
    public string Title { get; init; } 
    public string Description { get; init; } 
    public Priority Priority { get; init; }

    public UpdateTaskCommand(Guid id, string title, string description, Priority priority)
    {
        Id = id;
        Title = title;
        Description = description;
        Priority = priority;
    }
}
