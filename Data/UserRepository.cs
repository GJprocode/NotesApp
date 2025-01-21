using Dapper;
using System.Data;
using NotesAppBackend.Models;

namespace NotesAppBackend.Data
{
    public class UserRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            string query = "SELECT * FROM Users";
            return await _dbConnection.QueryAsync<User>(query);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            string query = "SELECT * FROM Users WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            string query = "SELECT * FROM Users WHERE Username = @Username";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });
        }

        public async Task<int> CreateUserAsync(User user)
        {
            string query = @"
                INSERT INTO Users (Username, PasswordHash, PasswordSalt, Email, CreatedAt, UpdatedAt)
                VALUES (@Username, @PasswordHash, @PasswordSalt, @Email, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await _dbConnection.ExecuteScalarAsync<int>(query, user);
        }
    }
}
