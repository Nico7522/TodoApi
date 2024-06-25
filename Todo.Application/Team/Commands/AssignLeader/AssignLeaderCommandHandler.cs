using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.AssignLeader
{


    internal class AssignLeaderCommandHandler : IRequestHandler<AssignLeaderCommand>
    {

        private readonly ITeamRepository _teamRepository;
        private readonly UserManager<UserEntity> _userManager;

        public AssignLeaderCommandHandler(ITeamRepository teamRepository, UserManager<UserEntity> userManager)
        {
            _teamRepository = teamRepository;
            _userManager = userManager;
        }
        public async System.Threading.Tasks.Task Handle(AssignLeaderCommand request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetById(request.TeamId);
            if (team is null) throw new NotFoundException("Team not found");

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null) throw new NotFoundException("User not found");

            if (team.LeaderId == user.Id) throw new BadRequestException("User is already leader of this team");

            if (user.TeamId != null) throw new BadRequestException("User is already in a team");

            team.LeaderId = user.Id;
            if(user.TeamId != team.Id) team.Users.Add(user);

            await _teamRepository.SaveChanges();
        }
    }
}
