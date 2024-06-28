using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Application.Users;
using Todo.Domain.Constants;
using Todo.Domain.Exceptions;

namespace Todo.Infrastructure.Authorization.Service;

public class TaskAuthorization : IAuthorization<TodoEntity>
{
    private readonly IUserContext _userContext;
    public TaskAuthorization(IUserContext userContext)
    {
        _userContext = userContext;
    }
    public bool Authorize(TodoEntity entity, RessourceOperation operation)
    {
        var user = _userContext.GetCurrentUser();
        var role = user!.Role;

        if (role == UserRole.SuperAdmin || role == UserRole.Admin) return true;

        if (operation == RessourceOperation.Update)
        {
            // Update task for team
            if (entity.Team?.LeaderId != user.Id) throw new ForbidException("Your not authorized");

            


            return false;
        }

        return false;
    }
}
