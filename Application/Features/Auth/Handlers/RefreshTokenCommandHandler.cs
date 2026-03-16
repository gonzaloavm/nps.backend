using Application.Contracts;
using Application.DTOs;
using Application.Features.Auth.Commands;
using Domain.Common;
using Domain.Entities.UserAggregate;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshSessionResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<Result<RefreshSessionResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
        {
            var oldTokenValue = request.RefreshToken;

            if (string.IsNullOrEmpty(oldTokenValue))
                return Result<RefreshSessionResponse>.Failure(new Error("401", "No existe el refresh token."));

            var (tokenRecord, session) = await _userRepository.GetTokenWithSessionAsync(oldTokenValue);

            if (tokenRecord == null || session == null || !session.IsActive)
                return Result<RefreshSessionResponse>.Failure(new Error("401", "Sesión inválida o inexistente."));

            if (tokenRecord.IsUsed || tokenRecord.IsRevoked)
            {
                session.IsActive = false;
                session.UpdateTimestamp();
                await _userRepository.UpdateSessionAsync(session);
                return Result<RefreshSessionResponse>.Failure(new Error("401", "Fraude detectado, sesión revocada."));
            }

            if (tokenRecord.ExpiresAt < DateTime.Now)
                return Result<RefreshSessionResponse>.Failure(new Error("401", "El refresh token ha expirado."));

            // *** VALIDACIÓN DE INACTIVIDAD (5 minutos desde LastActivityAt) ***
            if (DateTime.Now - session.LastActivityAt > TimeSpan.FromMinutes(5))
            {
                // Marcar sesión como inactiva
                session.IsActive = false;
                session.UpdateTimestamp();
                await _userRepository.UpdateSessionAsync(session);
                return Result<RefreshSessionResponse>.Failure(new Error("401", "Sesión expirada por inactividad."));
            }

            // Actualizar LastActivityAt por la actividad del refresh
            session.LastActivityAt = DateTime.Now;
            session.UpdateTimestamp();
            await _userRepository.UpdateSessionAsync(session);

            // Crear nuevo refresh token (vida 15 min)
            var newRefreshTokenValue = _tokenService.GenerateRefreshToken();
            var newRefreshToken = new RefreshToken
            {
                SessionId = session.Id,
                Token = newRefreshTokenValue,
                ExpiresAt = DateTime.Now.AddMinutes(15),
                IsUsed = false,
                IsRevoked = false
            };
            var newRefreshTokenId = await _userRepository.CreateRefreshTokenAsync(newRefreshToken);

            // Rotar el token viejo
            tokenRecord.IsUsed = true;
            tokenRecord.ReplacedByTokenId = newRefreshTokenId;
            tokenRecord.UpdateTimestamp();
            await _userRepository.UpdateRefreshTokenAsync(tokenRecord);

            // Establecer cookie con el nuevo refresh token
            _tokenService.SetRefreshTokenCookie(newRefreshTokenValue);

            // Obtener usuario y generar nuevo access token (5 min)
            var user = await _userRepository.GetByIdAsync(session.UserId);
            if (user == null)
                return Result<RefreshSessionResponse>.Failure(new Error("401", "Usuario no encontrado."));

            var jwt = _tokenService.GenerateToken(user, session.Id, minutes: 5); // <-- Pasar sessionId

            return Result<RefreshSessionResponse>.Success(new RefreshSessionResponse(jwt));
        }
    }
}
