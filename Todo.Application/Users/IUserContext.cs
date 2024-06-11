namespace Todo.Application.Users;

public interface IUserContext
{
    CurrentUser? GetCurrentUser();
}