using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Web;
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
    private readonly IEmailService _emailService;

    public RegisterCommandHandler(IMapper mapper, UserManager<UserEntity> userManager, IJwtHelper jwtHelper, IEmailSender emailSender, IEmailService emailService)
    {
        _mapper = mapper;
        _userManager = userManager;
        _jwtHelper = jwtHelper;
        _emailSender = emailSender;
        _emailService = emailService;
    }
    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<UserEntity>(request);

        var user = await _userManager.FindByEmailAsync(request.Email!);
        if (user is not null) throw new BadRequestException("Email already exist");


        var result = await _userManager.CreateAsync(entity, request.Password);
        if (!result.Succeeded) throw new ApiException("An error has occurred");

        var addRoleResult = await _userManager.AddToRoleAsync(entity, UserRole.User);
        if (!addRoleResult.Succeeded) throw new ApiException("An error has occurred");

        IList<Claim> baseClaims = await _jwtHelper.SetBaseClaims(entity);

        var addClaimsResult = await _userManager.AddClaimsAsync(entity, baseClaims);
        if (!addClaimsResult.Succeeded) throw new ApiException("An error has occurred");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(entity);
        var encodedToken = HttpUtility.UrlEncode(token);
        var confirmationLink = $"http://localhost:4200/confirm-email?userId={entity.Id}&token={encodedToken}";
        await _emailService.SendEmailAsync(entity.Email!, "Confirm Your Email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>;.", true);


        //await _emailSender.SendEmail(entity.Email!, "Account created", $"Welcome, to our team {entity.FirstName} {entity.LastName}");

        return true;
    }
}
