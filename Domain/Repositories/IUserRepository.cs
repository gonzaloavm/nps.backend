using Domain.Contracts.Common;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Repositories
{
    public interface IUserRepository : IRepositoryBase
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task UpdateAsync(User user);
        Task AddAsync(User user);
        Task<bool> HasVotedAsync(int userId);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
    }
}
