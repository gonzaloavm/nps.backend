using Domain.Common.Interfaces;
using Domain.Entities.DTOs;
using Domain.Entities.UserAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Repositories
{
    public interface IUserRepository : IRepositoryBase
    {
        Task<User?> GetByIdAsync(int userId);
        Task<User?> GetByUsernameAsync(string username);
        Task<int> AddAsync(User user);
        Task UpdateUserAsync(User user);
        Task<int> CreateSessionAsync(UserSession session);
        Task<int> CreateRefreshTokenAsync(RefreshToken token);
        Task<(RefreshToken? Token, UserSession? Session)> GetTokenWithSessionAsync(string tokenValue);
        Task UpdateRefreshTokenAsync(RefreshToken token);
        Task UpdateSessionAsync(UserSession session);
        Task UpdateSessionLastActivityAsync(int sessionId, DateTime lastActivity);
        Task<IEnumerable<VoterDto>> GetUsersWithVoteStatusAsync();
    }
}
