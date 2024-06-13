
using Microsoft.AspNetCore.Identity;

namespace Todo.Domain.Entities;

public class UserEntity : IdentityUser
{
    public DateOnly Birthdate { get; set; }

    public List<TodoEntity> Tasks { get; set; } = [];
    public TeamEntity? LeadedTeam { get; set; }


    public string? TeamId { get; set; }
    public TeamEntity? Team { get; set; }
}
