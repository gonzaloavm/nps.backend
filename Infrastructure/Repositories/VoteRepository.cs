using Dapper;
using Domain.Entities.DTOs;
using Domain.Entities.VoteAggregate;
using Domain.Repositories;
using System.Data;

namespace Infrastructure.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly IDbConnection _connection;

        public VoteRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> AddAsync(Vote vote)
        {
            const string sql = @"
            INSERT INTO Votes (UserId, Score, CreatedAt, UpdatedAt)
            VALUES (@UserId, @Score, @CreatedAt, @UpdatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int);";
            return await _connection.ExecuteScalarAsync<int>(sql, vote);
        }

        public async Task<bool> HasUserVotedAsync(int userId)
        {
            const string sql = "SELECT COUNT(1) FROM Votes WHERE UserId = @userId";
            var count = await _connection.ExecuteScalarAsync<int>(sql, new { userId });
            return count > 0;
        }

        // Este lo usaremos en el siguiente paso para el Admin
        public async Task<NpsStatisticsDto> GetNpsStatisticsAsync()
        {
            // Usamos COUNT(CASE...) para categorizar en un solo recorrido de la tabla
            const string sql = @"
            SELECT 
                COUNT(*) AS TotalVotes,
                COUNT(CASE WHEN Score >= 9 THEN 1 END) AS Promoters,
                COUNT(CASE WHEN Score <= 6 THEN 1 END) AS Detractors,
                COUNT(CASE WHEN Score IN (7, 8) THEN 1 END) AS Neutrals
            FROM Votes";

            // Dapper mapea las columnas a las propiedades del DTO interno
            return await _connection.QuerySingleAsync<NpsStatisticsDto>(sql);
        }
    }
}
