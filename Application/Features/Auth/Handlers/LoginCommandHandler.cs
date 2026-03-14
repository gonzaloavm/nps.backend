using Application.Contracts;
using Application.DTOs;
using Application.Features.Auth.Commands;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto?>
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

        public async Task<AuthResponseDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
                return null;

            // Verificar si la cuenta está bloqueada
            if (user.IsLocked)
            {
                if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                    return null; // Sigue bloqueado
                else
                {
                    // Desbloquear si pasó el tiempo
                    user.IsLocked = false;
                    user.FailedAttempts = 0;
                    user.LockoutEnd = null;
                    await _userRepository.UpdateAsync(user);
                }
            }

            // Verificar contraseña
            if (!_passwordHasher.VerifyPasswordHash(request.Password, user.PasswordHash))
            {
                // Incrementar intentos fallidos
                user.FailedAttempts++;
                if (user.FailedAttempts >= 3)
                {
                    user.IsLocked = true;
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15); // Bloquear por 15 minutos
                }
                await _userRepository.UpdateAsync(user);
                return null;
            }

            // Resetear intentos fallidos
            user.FailedAttempts = 0;
            user.LastActivity = DateTime.UtcNow;

            // Generar tokens
            var token = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userRepository.UpdateAsync(user);

            return new AuthResponseDto(token, refreshToken, user.Role, user.Username);
        }
    }
}
