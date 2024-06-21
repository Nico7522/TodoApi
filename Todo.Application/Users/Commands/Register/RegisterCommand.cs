using MediatR;

namespace Todo.Application.Users.Commands.Register;

public class RegisterCommand : IRequest<bool>
{
    public string Email { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public DateOnly HireDate { get; set; }
    public string Password { get; set; } = default!;
    public string PasswordConfirm { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public DateOnly BirthDate { get; set; }

}
