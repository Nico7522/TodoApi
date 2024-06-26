
using MediatR;
using Todo.Application.Users;
using Todo.Domain.Constants;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.UpdateTeamTask;

internal class UpdateTeamTaskCommandHandler : IRequestHandler<UpdateTeamTaskCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITodoRepository _todoRepository;
    private readonly IUserContext _userContext;

    public UpdateTeamTaskCommandHandler(ITeamRepository teamRepository, ITodoRepository todoRepository, IUserContext userContext)
    {
        _teamRepository = teamRepository;
        _todoRepository = todoRepository;
        _userContext = userContext;
    }
    public async System.Threading.Tasks.Task Handle(UpdateTeamTaskCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new NotFoundException("Task not found");

        if(currentUser!.Role == UserRole.Leader)
        {
            if (team.LeaderId != currentUser.Id) throw new ForbidException("Your not authorized");
        }


        // TODO: faire les modifs


    }
}
