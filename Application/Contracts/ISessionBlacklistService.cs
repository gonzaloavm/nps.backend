using Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts
{
    public interface ISessionBlacklistService
    {
        Task BlacklistSessionAsync(int sessionId, TimeSpan duration, CancellationToken ct = default);
        Task<bool> IsSessionBlacklistedAsync(int sessionId, CancellationToken ct = default);
    }
}
