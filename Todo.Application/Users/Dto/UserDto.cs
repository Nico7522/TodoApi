using Todo.Application.Task.Dto;

namespace Todo.Application.Users.Dto;

public class UserDto
{
    public string Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public DateOnly HireDate { get; set; }
    public DateOnly Birthdate { get; set; }
    public ICollection<TaskForUserDto> Tasks { get; set; } = new List<TaskForUserDto>();
    public Guid TeamId { get; set; }
}
