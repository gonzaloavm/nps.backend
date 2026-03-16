using Application.Contracts;
using Application.DTOs;
using Application.Features.Auth.Commands;
using Application.Validators;
using Domain.Common;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Common;

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
        [Tags("Autenticación")]
        [EndpointDescription("Autentica al usuario mediante credenciales y registra metadatos del dispositivo (IP, Dispositivo) y geolocalización.")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest loginDto,
            [FromServices] IDeviceInfoService deviceInfoService,
            [FromServices] IGeoLocationService geoService,
            CancellationToken cancellationToken)
        {
            var ip = deviceInfoService.GetIpAddress();
            var device = deviceInfoService.GetDeviceName();
            var location = await geoService.GetLocationAsync(ip, cancellationToken);

            var command = new LoginCommand(loginDto.Username, loginDto.Password, ip, device, location);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                if (result.Errors.Any(e => e.Code == "Unauthorized" || e.Code == "InvalidCredentials"))
                {
                    return Unauthorized(ProblemDetailsMapper.ToProblemDetails(result.Errors, StatusCodes.Status401Unauthorized, "Unauthorized"));
                }

                return BadRequest(ProblemDetailsMapper.ToProblemDetails(result.Errors, StatusCodes.Status400BadRequest, "Bad Request"));
            }

            return Ok(ApiResponse<LoginResponse>.Ok(result.Value));
        }

        [HttpPost("register")]
        [Authorize(Policy = "AdminOnly")]
        [Tags("Autenticación")]
        [EndpointDescription("Permite a un administrador dar de alta a nuevos usuarios (Votantes o Admins).")]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request,
            CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(request.Username, request.Password, request.Role);

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(ProblemDetailsMapper.ToProblemDetails(result.Errors, StatusCodes.Status400BadRequest, "Bad Request"));
            }

            return Ok(ApiResponse<RegisterResponse>.Ok(result.Value));
        }


        [HttpPost("refresh")]
        [Tags("Autenticación")]
        [EndpointDescription("Extiende la validez de la sesión utilizando el Refresh Token almacenado en las cookies HttpOnly.")]
        [ProducesResponseType(typeof(ApiResponse<RefreshSessionResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshSession(
            [FromServices] IDeviceInfoService deviceInfoService,
            CancellationToken cancellationToken)
        {
            // Extraemos el refresh token de la cookie
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                var missing = new[] { new Error("MissingRefreshToken", "No hay refresh token.", "refreshToken") };
                return Unauthorized(ProblemDetailsMapper.ToProblemDetails(missing, StatusCodes.Status401Unauthorized, "Unauthorized"));
            }

            var ip = deviceInfoService.GetIpAddress();

            var command = new RefreshTokenCommand(refreshToken, ip);

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                if (result.Errors.Any(e => e.Code == "InvalidRefreshToken" || e.Code == "ExpiredRefreshToken"))
                {
                    return Unauthorized(ProblemDetailsMapper.ToProblemDetails(result.Errors, StatusCodes.Status401Unauthorized, "Unauthorized"));
                }

                return BadRequest(ProblemDetailsMapper.ToProblemDetails(result.Errors, StatusCodes.Status400BadRequest, "Bad Request"));
            }

            return Ok(ApiResponse<RefreshSessionResponse>.Ok(result.Value));
        }


        [HttpPost("logout")]
        [Tags("Autenticación")]
        [EndpointDescription("Invalida el Refresh Token en el servidor y elimina la cookie del navegador.")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                var command = new LogoutCommand(refreshToken);
                var result = await _mediator.Send(command, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(ProblemDetailsMapper.ToProblemDetails(result.Errors, StatusCodes.Status400BadRequest, "Bad Request"));
                }
            }

            // Limpiar la cookie del lado del cliente (Navegador)
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            return Ok(ApiResponse<bool>.Ok(true));
        }

        /// <summary>
        /// Este endpoint al ser llamado, hace que el Middleware actualice el LastActivityAt de la sesión.
        /// El parámetro 't' se usa para evitar el caché del navegador.
        /// </summary>
        [HttpGet("verify-session")]
        [Authorize]
        [Tags("Autenticación")]
        [EndpointDescription("Valida si el token actual es vigente y actualiza la última actividad del usuario en el sistema.")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifySession([FromQuery] long? t = null)
        {
            // El parámetro 't' no se usa aquí, su sola presencia en la URL 
            // obliga al navegador a realizar una petición real al servidor.
            return Ok(ApiResponse<bool>.Ok(true));
        }
    }
}
