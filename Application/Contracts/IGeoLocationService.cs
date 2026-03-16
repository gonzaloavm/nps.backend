using Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts
{
    public interface IGeoLocationService : IServiceBase
    {
        Task<string> GetLocationAsync(string ip, CancellationToken ct = default);
    }
}
