namespace Todo.Domain.Repositories;

public interface IAuthRepository
{
    public Task<string> Login(string email, string password);
    public Task<bool> Register();
}
