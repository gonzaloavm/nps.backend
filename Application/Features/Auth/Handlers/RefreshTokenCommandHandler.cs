using Application.Contracts;
using Application.DTOs;
using Application.Features.Auth.Commands;
using Domain.Common;
using Domain.Entities.UserAggregate;
using Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

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
                return Result<RefreshSessionResponse>.Failure(new Error(BusinessErrorCodes.SessionExpired, "No existe el refresh token."));

            var (tokenRecord, session) = await _userRepository.GetTokenWithSessionAsync(oldTokenValue);

            if (tokenRecord == null || session == null || !session.IsActive)
                return Result<RefreshSessionResponse>.Failure(new Error(BusinessErrorCodes.SessionExpired, "Sesión inválida o inexistente."));

            // Invalidar sesión si el token ya fue utilizado o revocado previamente
            if (tokenRecord.IsUsed || tokenRecord.IsRevoked)
            {
                session.IsActive = false;
                session.UpdateTimestamp();
                await _userRepository.UpdateSessionAsync(session);
                return Result<RefreshSessionResponse>.Failure(new Error(BusinessErrorCodes.InvalidCredentials, "Fraude detectado, sesión revocada."));
            }

            if (tokenRecord.ExpiresAt < DateTime.Now)
                return Result<RefreshSessionResponse>.Failure(new Error(BusinessErrorCodes.SessionExpired, "El refresh token ha expirado."));

            // Verificar ventana de inactividad permitida
            if (DateTime.Now - session.LastActivityAt > TimeSpan.FromMinutes(5))
            {
                session.IsActive = false;
                session.UpdateTimestamp();
                await _userRepository.UpdateSessionAsync(session);
                return Result<RefreshSessionResponse>.Failure(new Error(BusinessErrorCodes.SecurityBreach, "Sesión expirada por inactividad."));
            }

            // Actualizar rastro de actividad tras validación exitosa
            session.LastActivityAt = DateTime.Now;
            session.UpdateTimestamp();
            await _userRepository.UpdateSessionAsync(session);

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

            // Rotar tokens para asegurar un único uso por ciclo
            tokenRecord.IsUsed = true;
            tokenRecord.ReplacedByTokenId = newRefreshTokenId;
            tokenRecord.UpdateTimestamp();
            await _userRepository.UpdateRefreshTokenAsync(tokenRecord);

            _tokenService.SetRefreshTokenCookie(newRefreshTokenValue);

            var user = await _userRepository.GetByIdAsync(session.UserId);
            if (user == null)
                return Result<RefreshSessionResponse>.Failure(new Error(BusinessErrorCodes.InvalidCredentials, "Usuario no encontrado."));

            var jwt = _tokenService.GenerateToken(user, session.Id, minutes: 5);

            return Result<RefreshSessionResponse>.Success(new RefreshSessionResponse(jwt));
        }
    }
}