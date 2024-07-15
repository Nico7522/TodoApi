using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Users.Queries.GetTasksNumberByUser;

public class GetTasksNumberByUserQueryHandler : IRequestHandler<GetTasksNumberByUserQuery, int>
{
    private readonly UserManager<UserEntity> _userManager;

    public GetTasksNumberByUserQueryHandler(UserManager<UserEntity> userManager)
    {
        _userManager = userManager;
    }
    public async Task<int> Handle(GetTasksNumberByUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.Include(t => t.Tasks).FirstOrDefaultAsync(u => u.Id == request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        return user.Tasks.Count();
    }
}
