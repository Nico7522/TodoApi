
using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.DeleteUser;

internal class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly UserManager<UserEntity> _userManager;

    public DeleteUserCommandHandler(ITeamRepository teamRepository, UserManager<UserEntity> userManager)
    {
        _teamRepository = teamRepository;
        _userManager = userManager;
    }
    public async System.Threading.Tasks.Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        if (user.TeamId != team.Id) throw new BadRequestException("User is not in team");

        team.Users.Remove(user);
        await _teamRepository.SaveChanges();

    }
}
