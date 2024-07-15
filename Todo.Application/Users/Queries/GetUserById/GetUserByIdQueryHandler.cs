
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Application.Users.Dto;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;

namespace Todo.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly IMapper _mapper;
    public GetUserByIdQueryHandler(UserManager<UserEntity> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user =  _userManager.Users.Include(u => u.Tasks).FirstOrDefault(u => u.Id == request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        var userDto = _mapper.Map<UserDto>(user);

        return userDto;


    }
}
