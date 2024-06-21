using MediatR;
using Todo.Application.Task.Dto;
using Todo.Domain.Entities;

namespace Todo.Application.Task.Queries.GetAllActiveTasks;

public class GetAllActiveTasksQuery : IRequest<IEnumerable<TodoDto>>
{
}
