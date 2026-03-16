using Domain.Common.Interfaces;
using Domain.Entities.UserAggregate;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Application.Contracts
{
    public interface ITokenService : IServiceBase
    {
        string GenerateToken(User user, int sessionId, int minutes);
        string GenerateRefreshToken();
        void SetRefreshTokenCookie(string token);

    }
}
