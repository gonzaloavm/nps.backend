using Application.Contracts;
using Application.DTOs;
using Application.Features.Auth.Commands;
using Domain.Common;
using Domain.Entities.UserAggregate;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(command.Username);

            if (user == null)
                return Result<LoginResponse>.Failure(new Error(BusinessErrorCodes.InvalidCredentials, "Credenciales inválidas."));

            // Control de Bloqueo
            if (user.IsLocked)
                return Result<LoginResponse>.Failure(new Error(BusinessErrorCodes.AccountLocked, "Cuenta bloqueada por exceso de intentos."));

            // Verificación de Password
            if (!_passwordHasher.VerifyPasswordHash(command.Password, user.PasswordHash))
            {
                user.AccessFailedCount++;
                user.UpdateTimestamp();

                if (user.AccessFailedCount >= 3)
                    user.IsLocked = true;

                await _userRepository.UpdateUserAsync(user);

                string errorMsg = user.IsLocked
                    ? "Cuenta bloqueada tras 3 intentos fallidos."
                    : "Credenciales inválidas.";

                return Result<LoginResponse>.Failure(new Error(BusinessErrorCodes.InvalidCredentials, errorMsg));
            }

            // Login Exitoso, reiniciar intentos
            user.AccessFailedCount = 0;
            user.UpdateTimestamp();
            await _userRepository.UpdateUserAsync(user);

            // Crear Sesión (Dura 5 min)
            var session = new UserSession
            {
                UserId = user.Id,
                LastActivityAt = DateTime.Now,
                IpAddress = command.Ip,
                Device = command.Device,
                Location = command.Location,
                IsActive = true
            };
            session.Id = await _userRepository.CreateSessionAsync(session);

            // Crear Refresh Token (Dura 5 min, ligado a la sesión)
            var refreshTokenValue = _tokenService.GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                SessionId = session.Id,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.Now.AddMinutes(15),
                IsRevoked = false
            };
            await _userRepository.CreateRefreshTokenAsync(refreshToken);

            // Registramos el refresh token en una cookie segura
            _tokenService.SetRefreshTokenCookie(refreshTokenValue);

            // Generar Access Token (Dura 1 min)
            var jwt = _tokenService.GenerateToken(user, session.Id, minutes: 5);

            return Result<LoginResponse>.Success(new LoginResponse(
                jwt,
                user.Role,
                user.Username
            ));
        }
    }
}
