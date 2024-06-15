

using FluentValidation;
using Todo.Domain.Constants;

namespace Todo.Application.Users.Commands.AssignRole;

public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{
    private readonly List<string> _roles = new List<string>()
    {
        UserRole.Leader, UserRole.Admin, UserRole.SuperAdmin, UserRole.User
    };
    public AssignRoleCommandValidator()
    {
        RuleFor(form => form.Role).NotEmpty().WithMessage("Required field").Must(r => _roles.Contains(r)).WithMessage("Bad role");
    }
}
