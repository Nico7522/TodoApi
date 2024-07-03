using Todo.Application.Task.Dto;
using Todo.Application.Users.Dto;
using Todo.Domain.Entities;

namespace Todo.Application.Team.Dto;

public class TeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; }
    public string? LeaderId { get; set; }
    public ICollection<TaskResumeDto> Tasks { get; set; } = new List<TaskResumeDto>();
    public ICollection<UserResumeDto> Users { get; set; } = new List<UserResumeDto>();

}
