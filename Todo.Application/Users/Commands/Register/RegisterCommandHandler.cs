using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Todo.Application.Email.Interfaces;
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
    private readonly IEmailSender _emailSender;
    public RegisterCommandHandler(IMapper mapper, UserManager<UserEntity> userManager, IJwtHelper jwtHelper, IEmailSender emailSender)
    {
        _mapper = mapper;
        _userManager = userManager;
        _jwtHelper = jwtHelper;
        _emailSender = emailSender;
    }
    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<UserEntity>(request);
        var user = await _userManager.FindByEmailAsync(request.Email!);
        if (user is not null) throw new BadRequestException("Bad request");


        var result = await _userManager.CreateAsync(entity, request.Password);
        if (!result.Succeeded) throw new BadRequestException("An error has occurred");

        await _userManager.AddToRoleAsync(entity, UserRole.User);
        IList<Claim> baseClaims = await _jwtHelper.SetBaseClaims(entity);
        var addClaimsResult = await _userManager.AddClaimsAsync(entity, baseClaims);

        if (!addClaimsResult.Succeeded) throw new BadRequestException("An error has occurred");
        await _emailSender.SendEmail(entity.Email!, "Account created", $"Welcome, to our team {entity.FirstName} {entity.LastName}");

        return true;
    }
}
