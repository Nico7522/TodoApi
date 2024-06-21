using Todo.Application.Task.Dto;
using Todo.Application.Users.Dto;
using Todo.Domain.Entities;

namespace Todo.Application.Team.Dto;

public class TeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; }
    public UserDto? Leader { get; set; }
    public ICollection<TodoDto> Tasks { get; set; } = new List<TodoDto>();
    public ICollection<UserDto> Users { get; set; } = new List<UserDto>();

}
