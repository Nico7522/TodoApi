using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;

namespace Todo.Application.Users.Commands.AssignRole;

internal class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand>
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly IValidator<AssignRoleCommand> _validator;
    public AssignRoleCommandHandler(UserManager<UserEntity> userManager, IValidator<AssignRoleCommand> validator)
    {
        _userManager = userManager;
        _validator = validator;
    }

    public async System.Threading.Tasks.Task Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {

        var result = _validator.Validate(request);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        var userRole = await _userManager.GetRolesAsync(user);

        if (userRole is not null)
        {
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, userRole);
            if (!removeRolesResult.Succeeded) throw new BadRequestException("A error has occured", 400);
        }
        var addRoleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!addRoleResult.Succeeded) throw new BadRequestException("A error has occured", 400);


    }
}
