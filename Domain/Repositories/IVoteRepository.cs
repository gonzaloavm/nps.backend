using Domain.Common.Interfaces;
using Domain.Entities;
using Domain.Entities.DTOs;
using Domain.Entities.VoteAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Repositories
{
    public interface IVoteRepository : IRepositoryBase
    {
        Task<int> AddAsync(Vote vote);
        Task<bool> HasUserVotedAsync(int userId);
        Task<NpsStatisticsDto> GetNpsStatisticsAsync();
    }
}
