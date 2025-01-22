using Dapper; // Lightweight ORM for database operations.
using System.Data; // Provides database connection and command interfaces.
using NotesBE.Models; // Includes the User model.

namespace NotesBE.Data
{
    public class UserRepository
    {
        private readonly IDbConnection _dbConnection; // Interface for database connection.

        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection; // Inject the database connection dependency.
        }

        // Retrieve all users from the database.
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            string query = "SELECT * FROM Users";
            return await _dbConnection.QueryAsync<User>(query);
        }

        // Retrieve a specific user by their unique ID.
        public async Task<User?> GetUserByIdAsync(int id)
        {
            string query = "SELECT * FROM Users WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
        }

        // Retrieve a user by their username (for authentication or validation).
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            string query = "SELECT * FROM Users WHERE Username = @Username";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });
        }

        // Retrieve a user by their email address (for registration or validation).
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            string query = "SELECT * FROM Users WHERE Email = @Email";
            return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Email = email });
        }

        // Insert a new user into the database and return their generated ID.
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
