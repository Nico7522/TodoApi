
using Microsoft.AspNetCore.Identity;

namespace Todo.Domain.Entities;

public class UserEntity : IdentityUser
{
    public DateOnly Birthdate { get; set; }
}
