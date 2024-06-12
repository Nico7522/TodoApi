using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.Users.Commands.Login;
using Todo.Application.Users.Commands.Register;

namespace Todo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            await _mediator.Send(command);
            return Ok(); 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            string token = await _mediator.Send(command);
            return Ok(token);
        }
    }
}
