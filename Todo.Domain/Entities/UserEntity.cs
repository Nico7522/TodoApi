
using Microsoft.AspNetCore.Identity;

namespace Todo.Domain.Entities;

public class UserEntity : IdentityUser
{
    public DateOnly Birthdate { get; set; }


    public ICollection<TodoEntity> Tasks { get; } = new List<TodoEntity>();


    public TeamEntity? LeadedTeam { get; set; }


    public Guid? TeamId { get; set; }
    public TeamEntity? Team { get; set; }
}
