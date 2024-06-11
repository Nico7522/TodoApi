namespace Todo.Application.Users;

public record CurrentUser
{
    public string Id { get; } = default!;
    public string Email { get; }
    public string Role { get; }
    public DateOnly Birthdate { get; } = default!;
    public CurrentUser(string id, string email, string role, DateOnly birthdate)
    {
        Id = id;
        Email = email; 
        Role = role; 
        Birthdate = birthdate;
    }
        public bool HasRole(string role) => role == Role;
}
