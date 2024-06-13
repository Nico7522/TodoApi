
using MediatR;
using Todo.Application.Task.Dto;
using Todo.Domain.Entities;

namespace Todo.Application.Task.Queries.GetTaskById;

public class GetTaskByIdQuery : IRequest<TodoDto>
{
    public string TaskId { get; init; }
    public GetTaskByIdQuery(string taskId)
    {
        TaskId = taskId;
    }
}
