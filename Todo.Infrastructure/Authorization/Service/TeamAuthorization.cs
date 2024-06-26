using Todo.Application.Users;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Constants;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Infrastructure.Authorization.Service;

public class TeamAuthorization : IAuthorization<TeamEntity>
{
    private readonly IUserContext _userContext;
    public TeamAuthorization(IUserContext userContext)
    {
        _userContext = userContext;
    }
    public bool Authorize(TeamEntity entity, RessourceOperation operation, object? data)
    {
        var user = _userContext.GetCurrentUser();
        var role = user!.Role;

        if(role == UserRole.SuperAdmin || role == UserRole.Admin) return true;

        if (operation == RessourceOperation.Read) return true;


        if(operation == RessourceOperation.Create || operation == RessourceOperation.Delete || operation == RessourceOperation.Update)
        {
            var userId = entity.Users.FirstOrDefault(u => u.Id == user.Id);
            if (userId is null) return false;
            if (role == UserRole.Leader && user.Id == entity.LeaderId) return true;

            return false;
        }

        return false;

    }
}
