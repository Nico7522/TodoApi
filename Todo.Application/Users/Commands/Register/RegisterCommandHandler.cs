
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Todo.Domain.Constants;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;
using Todo.Domain.Security;

namespace Todo.Application.Users.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
{

    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;
    public RegisterCommandHandler(IAuthRepository authRepository, IMapper mapper)
    {
       _authRepository = authRepository;
       _mapper = mapper;
    }
    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<UserEntity>(request);
        return await _authRepository.Register(entity, request.Password);
    }
}
