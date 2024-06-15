﻿using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
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

        var userRoles = await _userManager.GetRolesAsync(user);

        if (userRoles.Any())
        {
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (!removeRolesResult.Succeeded) throw new BadRequestException("A error has occured", 400);
        }
        var addRoleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!addRoleResult.Succeeded) throw new BadRequestException("A error has occured", 400);

        var userClaims = await _userManager.GetClaimsAsync(user);

        var oldRoleClaim = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        if (oldRoleClaim is null)
        {
            var addClaimResult = await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, request.Role));
            if (!addClaimResult.Succeeded) throw new BadRequestException("Error", 400);
        }
        else
        {
            var replaceClaimResult = await _userManager.ReplaceClaimAsync(user, oldRoleClaim, new Claim(ClaimTypes.Role, request.Role));
            if (!replaceClaimResult.Succeeded) throw new BadRequestException("Error", 400);
        }

    }
}
