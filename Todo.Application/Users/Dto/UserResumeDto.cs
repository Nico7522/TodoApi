
namespace Todo.Application.Users.Dto;

public class UserResumeDto
{
    public string Id { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Firstname { get; set; } = default!;
    public string Lastname { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public DateOnly Hiredate { get; set; }
    public DateOnly Birthdate { get; set; }
}
