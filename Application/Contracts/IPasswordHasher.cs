using Domain.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts
{
    public interface IPasswordHasher : IServiceBase
    {
        string HashPassword(string password);
        bool VerifyPasswordHash(string password, string passwordHash);
    }
}
