using Application.DTOs;
using Application.Features.Auth.Commands;
using Application.Validators;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var validator = new LoginValidator();
            var validationResult = await validator.ValidateAsync(loginDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = new LoginCommand (loginDto.Username, loginDto.Password);
            var result = await _mediator.Send(command);

            if (result == null)
                return Unauthorized(new { message = "Invalid credentials or account locked." });

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            if (result == null)
                return Unauthorized(new { message = "Invalid refresh token." });
            return Ok(result);
        }
    }
}
