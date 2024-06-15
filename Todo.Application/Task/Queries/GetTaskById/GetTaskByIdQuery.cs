
using MediatR;
using Todo.Application.Task.Dto;
using Todo.Domain.Entities;

namespace Todo.Application.Task.Queries.GetTaskById;

public class GetTaskByIdQuery : IRequest<TodoDto>
{
    public Guid TaskId { get; init; }
    public GetTaskByIdQuery(Guid taskId)
    {
        TaskId = taskId;
    }
}
