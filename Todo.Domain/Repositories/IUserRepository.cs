
using Todo.Domain.Entities;

namespace Todo.Domain.Repositories;

public interface IUserRepository
{
    Task<int> GetTaskNumberByUser();
}
