using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Application.Team.Dto;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;

namespace Todo.Application.Users.Queries.GetTeamByUser;

public class GetTeamByUserQueryHandler : IRequestHandler<GetTeamByUserQuery, TeamDto>
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly IMapper _mapper;
    public GetTeamByUserQueryHandler(UserManager<UserEntity> userManager, IMapper mapper)
    {
        _userManager = userManager; 
        _mapper = mapper;
    }
    public async Task<TeamDto> Handle(GetTeamByUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .Include(u => u.Team).ThenInclude(t => t.Users)
            .Include(u => u.Team).ThenInclude(t => t.Tasks)
            .FirstOrDefaultAsync(u => u.Id == request.UserId);
       
        if (user is null) throw new NotFoundException("User not found");

        var results = _mapper.Map<TeamDto>(user.Team);

        return results;
    }
}
