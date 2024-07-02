namespace Todo.Domain.Entities;

public class TeamEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? LeaderId { get; set; }
    public bool IsActive { get; set; }
    public UserEntity? Leader { get; set; }
    public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
    public ICollection<TodoEntity> Tasks { get; set; } = new List<TodoEntity>();
}
