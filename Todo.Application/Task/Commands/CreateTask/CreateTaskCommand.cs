using MediatR;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Application.Task.Commands.CreateTask;

public class CreateTaskCommand : IRequest<string>
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Priority Priority { get; set; }


}
