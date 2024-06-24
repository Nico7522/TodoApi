using Todo.Application.Users;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Constants;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Infrastructure.Authorization.Service;

public class TeamAuthorizationService : ITeamAuthorizationService
{
    private readonly IUserContext _userContext;
    public TeamAuthorizationService(IUserContext userContext)
    {
        _userContext = userContext;
    }
    public bool Authorize(TeamEntity team, RessourceOperation operation)
    {
        var user = _userContext.GetCurrentUser();
        var role = user!.Role;

        if (operation == RessourceOperation.Create || operation == RessourceOperation.Update || operation == RessourceOperation.Delete)
        {
            if (role == UserRole.User) return false;
            if (role == UserRole.Admin || role == UserRole.SuperAdmin) return true;
            if (role == UserRole.Leader && user.Id == team.LeaderId) return true;
        }

        if (operation == RessourceOperation.Read) return true;


        return false;

    }
}
