using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;

namespace Todo.Application.Users.Commands.UnassignRole;

internal class UnassignRoleCommandHandler : IRequestHandler<UnassignRoleCommand>
{
    private readonly UserManager<UserEntity> _userManager;

    public UnassignRoleCommandHandler(UserManager<UserEntity> userManager)
    {
        _userManager = userManager;
    }
    public async System.Threading.Tasks.Task Handle(UnassignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count() <= 0) throw new BadRequestException("User has no role", 400);

        var roleDeletedResult = await _userManager.RemoveFromRolesAsync(user, roles);
        if (!roleDeletedResult.Succeeded) throw new BadRequestException("Role has not been deleted", 400);

        var userClaims = await _userManager.GetClaimsAsync(user);

        var roleClaim = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        if(roleClaim is not null)
        {
            var deletedRoleClaimResult = await _userManager.RemoveClaimAsync(user, roleClaim);
            if (!deletedRoleClaimResult.Succeeded) throw new BadRequestException("Error", 400);
        }
    }
}
