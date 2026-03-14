using Dapper;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly DapperContext _context;

        public VoteRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Vote vote)
        {
            var query = @"
                INSERT INTO Votes (UserId, Score, VotedAt)
                VALUES (@UserId, @Score, @VotedAt);
                SELECT CAST(SCOPE_IDENTITY() as int)";
            using var connection = _context.CreateConnection();
            var id = await connection.ExecuteScalarAsync<int>(query, vote);
            vote.Id = id;
        }

        public async Task<IEnumerable<Vote>> GetAllAsync()
        {
            var query = "SELECT * FROM Votes";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Vote>(query);
        }

        public async Task<Vote?> GetByUserIdAsync(int userId)
        {
            var query = "SELECT * FROM Votes WHERE UserId = @UserId";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Vote>(query, new { UserId = userId });
        }

        public async Task<int> GetDetractorsCountAsync()
        {
            var query = "SELECT COUNT(1) FROM Votes WHERE Score <= 6";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query);
        }

        public async Task<int> GetNeutralsCountAsync()
        {
            var query = "SELECT COUNT(1) FROM Votes WHERE Score = 7 OR Score = 8";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query);
        }

        public async Task<int> GetPromotersCountAsync()
        {
            var query = "SELECT COUNT(1) FROM Votes WHERE Score >= 9";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query);
        }

        public async Task<int> GetTotalVotesAsync()
        {
            var query = "SELECT COUNT(1) FROM Votes";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query);
        }
    }
}
