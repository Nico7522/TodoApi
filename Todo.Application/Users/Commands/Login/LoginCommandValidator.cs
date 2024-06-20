using FluentValidation;
using Todo.Application.Utility;

namespace Todo.Application.Users.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(f => f.Email)
            .NotEmpty()
            .WithMessage(Helpers.SetRequiredErrorMessage("Email"))
            .EmailAddress()
            .WithMessage("Email is not valid");
        RuleFor(f => f.Password)
            .NotEmpty()
            .WithMessage(Helpers.SetRequiredErrorMessage("Password"))
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            .WithMessage("Password not match the requirement");
    }
}
