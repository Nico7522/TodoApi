using FluentValidation;
using Todo.Application.Utility;
namespace Todo.Application.Users.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
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
        RuleFor(f => f.FirstName)
            .NotEmpty()
            .WithMessage(Helpers.SetRequiredErrorMessage("First name"));
        RuleFor(f => f.LastName)
            .NotEmpty()
            .WithMessage(Helpers.SetRequiredErrorMessage("Last name"));
        RuleFor(f => f.PhoneNumber)
            .NotEmpty()
            .WithMessage(Helpers.SetRequiredErrorMessage("Phone number"))
            .Matches(@"^\d{9,9}$")
            .WithMessage("Invalid phone number");
        RuleFor(f => f.BirthDate)
            .NotEmpty()
            .WithMessage(Helpers.SetRequiredErrorMessage("Birthdate"));
        RuleFor(f => f.HireDate)
          .NotEmpty()
          .WithMessage(Helpers.SetRequiredErrorMessage("Hiredate"));
    }
}
