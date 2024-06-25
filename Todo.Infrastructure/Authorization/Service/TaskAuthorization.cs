using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Application.Users;
using Todo.Domain.Constants;

namespace Todo.Infrastructure.Authorization.Service;

public class TaskAuthorization : IAuthorization<TodoEntity>
{
    private readonly IUserContext _userContext;
    public TaskAuthorization(IUserContext userContext)
    {
        _userContext = userContext;
    }
    public bool Authorize(TodoEntity entity, RessourceOperation operation, object? data)
    {
        var user = _userContext.GetCurrentUser();
        var role = user!.Role;

        if (operation == RessourceOperation.Update)
        {
            if (role == UserRole.SuperAdmin || role == UserRole.Admin) return true;

            if (data != null)
            {
                var taskUserId = data.ToString();
                if (taskUserId != user.Id) return false;

                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }
}
