using MediatR;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Application.Task.Commands.UpdateTask;

public class UpdateTaskCommand : IRequest
{
    public Guid Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Priority Priority { get; set; }
}
