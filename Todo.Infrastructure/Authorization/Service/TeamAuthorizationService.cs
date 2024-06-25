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
    public bool Authorize(TeamEntity team, RessourceOperation operation, object? data)
    {
        var user = _userContext.GetCurrentUser();
        var role = user!.Role;
        Console.WriteLine("ici");

        if (operation == RessourceOperation.Read) return true;

        if (operation == RessourceOperation.Delete)
        {
            if (data != null)
            {
                var userIdToRemove = data;
                if (role == UserRole.Leader && user.Id == userIdToRemove.ToString()) return false;
            }
            else
            {
                return false;
            }
        };

        if (role == UserRole.User) return false;
        if (role == UserRole.Admin || role == UserRole.SuperAdmin) return true;
        if (role == UserRole.Leader && user.Id == team.LeaderId) return true;


        return false;

    }
}
