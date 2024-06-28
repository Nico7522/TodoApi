using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Domain.AuthorizationInterfaces;

public interface IAuthorization<T> where T : class
{
    bool Authorize(T entity, RessourceOperation operation);
}