using Todo.Domain.Enums;

namespace Todo.Domain.Entities;

public class TodoEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateOnly CreationDate { get; set; }
    public DateOnly? ClosingDate { get; set; }
    public Piority Priority { get; set; }
    public bool IsComplete { get; set; }

    public string? UserId { get; set; }
    public UserEntity? User { get; set; }

    public string? TeamId { get; set; }
    public TeamEntity? Team { get; set; }
}
