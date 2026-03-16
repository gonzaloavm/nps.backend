using Application.Contracts;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class SessionBlacklistService : ISessionBlacklistService
    {
        private readonly IDistributedCache _cache;
        private const string KeyPrefix = "revoked_session:";

        public SessionBlacklistService(IDistributedCache cache) => _cache = cache;

        public async Task BlacklistSessionAsync(int sessionId, TimeSpan duration, CancellationToken ct = default)
        {
            await _cache.SetStringAsync($"{KeyPrefix}{sessionId}", "true", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = duration
            }, ct);
        }

        public async Task<bool> IsSessionBlacklistedAsync(int sessionId, CancellationToken ct = default)
        {
            var result = await _cache.GetStringAsync($"{KeyPrefix}{sessionId}", ct);
            return result != null;
        }
    }
}
