
using Todo.Application.Users;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Infrastructure.Authorization.Service;

public class UserAuthorization : IAuthorization<UserEntity>
{
    private readonly IUserContext _userContext;
    public UserAuthorization(IUserContext userContext)
    {
        _userContext = userContext;
    }
    public bool Authorize(UserEntity entity, RessourceOperation operation)
    {
        var user = _userContext.GetCurrentUser();
        var role = user!.Role;

        if (operation == RessourceOperation.Update)
        {



        }

        return false;
    }
}
