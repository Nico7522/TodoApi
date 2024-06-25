using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Domain.AuthorizationInterfaces;

public interface ITeamAuthorizationService
{
    bool Authorize(TeamEntity team, RessourceOperation operation, object? data);
}