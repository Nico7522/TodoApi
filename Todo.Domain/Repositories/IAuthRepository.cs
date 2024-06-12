using Todo.Domain.Entities;

namespace Todo.Domain.Repositories;

public interface IAuthRepository
{
    public Task<string> Login(string email, string password);
    public Task<bool> Register(UserEntity entity, string password);
}
