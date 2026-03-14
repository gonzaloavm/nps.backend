using Dapper;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            var query = @"
                INSERT INTO Users (Username, PasswordHash, Role, IsLocked, FailedAttempts, LockoutEnd, LastActivity, RefreshToken, RefreshTokenExpiryTime)
                VALUES (@Username, @PasswordHash, @Role, @IsLocked, @FailedAttempts, @LockoutEnd, @LastActivity, @RefreshToken, @RefreshTokenExpiryTime);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            using var connection = _context.CreateConnection();
            var id = await connection.ExecuteScalarAsync<int>(query, user);
            user.Id = id;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM Users WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<User>(query, new { Id = id });
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            var query = "SELECT * FROM Users WHERE RefreshToken = @RefreshToken";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<User>(query, new { RefreshToken = refreshToken });
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var query = "SELECT * FROM Users WHERE Username = @Username";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<User>(query, new { Username = username });
        }

        public async Task<bool> HasVotedAsync(int userId)
        {
            var query = "SELECT COUNT(1) FROM Votes WHERE UserId = @UserId";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(query, new { UserId = userId });
            return count > 0;
        }

        public async Task UpdateAsync(User user)
        {
            var query = @"
                UPDATE Users 
                SET PasswordHash = @PasswordHash, 
                    Role = @Role, 
                    IsLocked = @IsLocked, 
                    FailedAttempts = @FailedAttempts, 
                    LockoutEnd = @LockoutEnd, 
                    LastActivity = @LastActivity, 
                    RefreshToken = @RefreshToken, 
                    RefreshTokenExpiryTime = @RefreshTokenExpiryTime
                WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, user);
        }
    }
}
