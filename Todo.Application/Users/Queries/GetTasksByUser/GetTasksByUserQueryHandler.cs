using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Application.Task.Dto;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Users.Queries.GetTasksByUser;

internal class GetTasksByUserQueryHandler : IRequestHandler<GetTasksByUserQuery, IEnumerable<TodoDto>>
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public GetTasksByUserQueryHandler(IUserRepository userRepository, IMapper mapper, UserManager<UserEntity> userManager)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<IEnumerable<TodoDto>> Handle(GetTasksByUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetTasksByUser(request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        var userTasksDto = _mapper.Map<IEnumerable<TodoDto>>(user.Tasks);
        return userTasksDto;

    }
}
