
using Todo.Domain.Entities;

namespace Todo.Domain.Repositories;

public interface IUserRepository
{
    Task<int> GetTaskNumberByUser();
    Task<UserEntity?> GetUserWithTeam(string userId);
}
