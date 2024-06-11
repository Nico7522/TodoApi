using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Todo.Application.Users;
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUser? GetCurrentUser()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user is null) throw new InvalidOperationException("User not found");

        if (user.Identity is null || !user.Identity.IsAuthenticated) return null;

        var id = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        var email = user.FindFirst(c => c.Type == ClaimTypes.Email)!.Value;
        var role = user.FindFirst(c => c.Type == ClaimTypes.Role)!.Value;
        var birthdateString = user.FindFirst(c => c.Type == "Birthdate")?.Value;
        var birthdate = DateOnly.ParseExact(birthdateString!, "yyyy-MM-dd");

        return new CurrentUser(id, email, role, birthdate);

    }
}
