namespace Todo.Application.Users;

public record CurrentUser
{
    public string Id { get; } = default!;
    public string Email { get; }
    public string Role { get; }
    public CurrentUser(string id, string email, string role)
    {
        Id = id;
        Email = email; 
        Role = role; 
    }
        public bool HasRole(string role) => role == Role;
}
