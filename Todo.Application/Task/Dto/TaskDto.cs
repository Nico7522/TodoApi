using Todo.Application.Users.Dto;
using Todo.Domain.Enums;

namespace Todo.Application.Task.Dto;

public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateOnly CreationDate { get; set; }
    public DateOnly? ClosingDate { get; set; }
    public TimeOnly Duration { get; set; }
    public Priority Priority { get; set; }
    public bool IsComplete { get; set; }
    public string? UserId { get; set; }
    public Guid TeamId { get; set; }
}
