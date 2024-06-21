
using Todo.Domain.Entities;

namespace Todo.Domain.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> GetTasksByUser(string userId);
    Task<bool> AssignTaskByUser(UserEntity user, TodoEntity task);
}
