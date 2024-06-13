using Todo.Domain.Enums;

namespace Todo.Application.Task.Dto;

public class TodoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateOnly CreationDate { get; set; }
    public DateOnly? ClosingDate { get; set; }
    public Piority Priority { get; set; }
    public bool IsComplete { get; set; }
}
