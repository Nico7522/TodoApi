using AutoMapper;
using MediatR;
using Todo.Application.Team.Dto;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Queries.GetAllTeams;

public class GetAllTeamQueryHandler : IRequestHandler<GetAllTeamQuery, IEnumerable<TeamDto>>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IMapper _mapper;
    public GetAllTeamQueryHandler(ITeamRepository teamRepository, IMapper mapper)
    {
        _teamRepository = teamRepository;
        _mapper = mapper;
    }
    public async Task<IEnumerable<TeamDto>> Handle(GetAllTeamQuery request, CancellationToken cancellationToken)
    {
        var teams = await _teamRepository.GetAll(request.IsActive);
        var teamsDto = _mapper.Map<IEnumerable<TeamDto>>(teams);
        return teamsDto;
    }
}
