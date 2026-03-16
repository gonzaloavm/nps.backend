using Application.Contracts;
using Application.Features.Auth.Commands;
using Domain.Common;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Handlers
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public LogoutCommandHandler(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return Result<bool>.Success(true); // Ya está deslogueado

            // Obtener la dupla Token/Sesión
            var (tokenRecord, session) = await _userRepository.GetTokenWithSessionAsync(request.RefreshToken);

            if (tokenRecord != null)
            {
                // Revocar el Refresh Token
                tokenRecord.IsRevoked = true;
                tokenRecord.UpdateTimestamp();
                await _userRepository.UpdateRefreshTokenAsync(tokenRecord);
            }

            if (session != null)
            {
                // Matar la sesión
                session.IsActive = false;
                session.UpdateTimestamp();
                await _userRepository.UpdateSessionAsync(session);
            }

            return Result<bool>.Success(true);
        }
    }
}
