using Dapper;
using Domain.Entities.DTOs;
using Domain.Entities.UserAggregate;
using Domain.Repositories;
using System.Data;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            const string sql = "SELECT * FROM Users WHERE Id = @userId";
            return await _connection.QueryFirstOrDefaultAsync<User>(sql, new { userId });
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            const string sql = "SELECT * FROM Users WHERE Username = @username";
            return await _connection.QueryFirstOrDefaultAsync<User>(sql, new { username });
        }

        public async Task<int> AddAsync(User user)
        {
            const string sql = @"
            INSERT INTO Users (Username, PasswordHash, Role, AccessFailedCount, IsLocked, CreatedAt, UpdatedAt)
            VALUES (@Username, @PasswordHash, @Role, @AccessFailedCount, @IsLocked, @CreatedAt, @UpdatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int);";

            return await _connection.ExecuteScalarAsync<int>(sql, user);
        }

        public async Task UpdateUserAsync(User user)
        {
            const string sql = @"
            UPDATE Users SET 
                AccessFailedCount = @AccessFailedCount, 
                IsLocked = @IsLocked, 
                UpdatedAt = @UpdatedAt 
            WHERE Id = @Id";
            await _connection.ExecuteAsync(sql, user);
        }

        public async Task<int> CreateSessionAsync(UserSession session)
        {
            const string sql = @"
                INSERT INTO UserSessions (UserId, IpAddress, Device, Location, IsActive, LastActivityAt, CreatedAt, UpdatedAt)
                VALUES (@UserId, @IpAddress, @Device, @Location, @IsActive, @LastActivityAt, GETUTCDATE(), GETUTCDATE());
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await _connection.ExecuteScalarAsync<int>(sql, session);
        }

        public async Task<int> CreateRefreshTokenAsync(RefreshToken token)
        {
            const string sql = @"
            INSERT INTO RefreshTokens (SessionId, Token, ExpiresAt, IsRevoked, IsUsed, CreatedAt, UpdatedAt)
            VALUES (@SessionId, @Token, @ExpiresAt, @IsRevoked, @IsUsed, @CreatedAt, @UpdatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await _connection.ExecuteScalarAsync<int>(sql, token);
        }

        public async Task<(RefreshToken? Token, UserSession? Session)> GetTokenWithSessionAsync(string tokenValue)
        {
            const string sql = @"
            SELECT t.*, s.* FROM RefreshTokens t
            INNER JOIN UserSessions s ON t.SessionId = s.Id
            WHERE t.Token = @tokenValue";

            var result = await _connection.QueryAsync<RefreshToken, UserSession, (RefreshToken, UserSession)>(
                sql,
                (token, session) => (token, session),
                new { tokenValue },
                splitOn: "Id"
            );

            return result.FirstOrDefault();
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken token)
        {
            const string sql = @"
            UPDATE RefreshTokens SET 
                IsRevoked = @IsRevoked, 
                IsUsed = @IsUsed, 
                UpdatedAt = @UpdatedAt,
                ReplacedByTokenId = @ReplacedByTokenId
            WHERE Id = @Id";
            await _connection.ExecuteAsync(sql, token);
        }

        public async Task UpdateSessionAsync(UserSession session)
        {
            const string sql = @"
            UPDATE UserSessions SET 
                LastActivityAt = @LastActivityAt,
                IsActive = @IsActive, 
                UpdatedAt = @UpdatedAt 
            WHERE Id = @Id";
            await _connection.ExecuteAsync(sql, session);
        }

        public async Task<IEnumerable<VoterDto>> GetUsersWithVoteStatusAsync()
        {
            const string sql = @"
            SELECT 
                u.Id, 
                u.Username, 
                u.Role, 
                u.CreatedAt,
                CASE WHEN v.UserId IS NOT NULL THEN 1 ELSE 0 END AS HasVoted
            FROM Users u
            LEFT JOIN Votes v ON u.Id = v.UserId
            WHERE u.Role = 'Voter'
            ORDER BY u.CreatedAt DESC";

            return await _connection.QueryAsync<VoterDto>(sql);
        }

        public async Task UpdateSessionLastActivityAsync(int sessionId, DateTime lastActivity)
        {
            const string sql = "UPDATE UserSessions SET LastActivityAt = @lastActivity, UpdatedAt = @lastActivity WHERE Id = @sessionId";
            await _connection.ExecuteAsync(sql, new { lastActivity, sessionId });
        }
    }
}