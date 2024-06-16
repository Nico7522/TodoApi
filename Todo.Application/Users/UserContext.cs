﻿using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Todo.Domain.Exceptions;

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

        if (user is null) throw new NotFoundException("User not found");

        if (user.Identity is null || !user.Identity.IsAuthenticated) throw new NotFoundException("User not found");

        var id = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        var email = user.FindFirst(c => c.Type == ClaimTypes.Email)!.Value;
        var role = user.FindFirst(c => c.Type == ClaimTypes.Role)!.Value;

        return new CurrentUser(id, email, role);

    }
}
