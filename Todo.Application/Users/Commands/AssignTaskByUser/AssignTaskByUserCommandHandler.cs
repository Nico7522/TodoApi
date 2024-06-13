﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Users.Commands.AssignTaskByUser;

internal class AssignTaskByUserCommandHandler : IRequestHandler<AssignTaskByUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<UserEntity> _userManager;
    private readonly ITodoRepository _todoRepository;

    public AssignTaskByUserCommandHandler(IUserRepository userRepository, UserManager<UserEntity> userMananger, ITodoRepository todoRepository )
    { 
        _userRepository = userRepository;
        _userManager = userMananger;
        _todoRepository = todoRepository;
    }
    public async Task<bool> Handle(AssignTaskByUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new ApiErrorException("User not found", 404);

        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new ApiErrorException("Task not found", 404);

        var result = await _userRepository.AssignTaskByUser(user, task);
        if (!result) throw new ApiErrorException("Error", 400);


        return true;
        // TODO: Add GetTaskBuId
        //var task = await 
    }
}
