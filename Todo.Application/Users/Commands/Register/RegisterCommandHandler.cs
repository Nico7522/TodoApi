using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Todo.Domain.Constants;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Security;

namespace Todo.Application.Users.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
{

    private readonly IMapper _mapper;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IJwtHelper _jwtHelper;
    public RegisterCommandHandler(IMapper mapper, UserManager<UserEntity> userManager, IJwtHelper jwtHelper)
    {
       _mapper = mapper;
       _userManager = userManager;
       _jwtHelper = jwtHelper;
    }
    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<UserEntity>(request);
        var user = await _userManager.FindByEmailAsync(request.Email!);
        if (user is not null) throw new BadRequestException("Bad request", 400);


        var result = await _userManager.CreateAsync(entity, request.Password);
        if (!result.Succeeded) throw new BadRequestException("An error has occurred", 400);
        await _userManager.AddToRoleAsync(entity, UserRole.User);
        IList<Claim> baseClaims = await _jwtHelper.SetBaseClaims(entity);
        var addClaimsResult = await _userManager.AddClaimsAsync(entity, baseClaims);
        if (!addClaimsResult.Succeeded) throw new BadRequestException("An error has occurred", 400);

        return true;
    }
}
