using FluentValidation;

namespace Todo.Application.Team.Commands.CreateTeam;

public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(t => t.Name).NotEmpty().WithMessage("Name is required");
    }
}
