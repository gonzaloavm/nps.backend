using Domain.Contracts.Common;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Repositories
{
    public interface IVoteRepository : IRepositoryBase
    {
        Task AddAsync(Vote vote);
        Task<IEnumerable<Vote>> GetAllAsync();
        Task<int> GetTotalVotesAsync();
        Task<int> GetPromotersCountAsync();
        Task<int> GetDetractorsCountAsync();
        Task<int> GetNeutralsCountAsync();
        Task<Vote?> GetByUserIdAsync(int userId);
    }
}
