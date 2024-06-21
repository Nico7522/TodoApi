namespace Todo.Application.Users.Dto;

public class UserDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public DateOnly HireDate { get; set; }
    public DateOnly Birthdate { get; set; }
}
