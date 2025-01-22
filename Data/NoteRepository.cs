using Dapper; // Lightweight ORM for interacting with the database.
using System.Data; // Provides access to database connection and commands.
using NotesBE.Models; // Includes the Note model.

namespace NotesBE.Data
{
    public class NoteRepository
    {
        private readonly IDbConnection _dbConnection; // Database connection interface.

        public NoteRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection; // Inject the database connection dependency.
        }

        // Fetch all notes for a specific user, ordered by creation date (most recent first).
        public async Task<IEnumerable<Note>> GetNotesByUserIdAsync(int userId)
        {
            string query = "SELECT * FROM Notes WHERE UserId = @UserId ORDER BY CreatedAt DESC";
            return await _dbConnection.QueryAsync<Note>(query, new { UserId = userId });
        }

        // Retrieve a specific note by its unique ID.
        public async Task<Note?> GetNoteByIdAsync(int id)
        {
            string query = "SELECT * FROM Notes WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Note>(query, new { Id = id });
        }

        // Insert a new note into the database and return the generated ID.
        public async Task<int> CreateNoteAsync(Note note)
        {
            string query = @"
                INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
                VALUES (@UserId, @Title, @Content, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await _dbConnection.ExecuteScalarAsync<int>(query, note);
        }

        // Update an existing note's title, content, and timestamp.
        public async Task<bool> UpdateNoteAsync(Note note)
        {
            string query = @"
                UPDATE Notes
                SET Title = @Title, Content = @Content, UpdatedAt = @UpdatedAt
                WHERE Id = @Id AND UserId = @UserId;
            ";
            int rowsAffected = await _dbConnection.ExecuteAsync(query, note);
            return rowsAffected > 0;
        }

        // Delete a note by its ID and associated UserId.
        public async Task<bool> DeleteNoteAsync(int id, int userId)
        {
            string query = "DELETE FROM Notes WHERE Id = @Id AND UserId = @UserId";
            int rowsAffected = await _dbConnection.ExecuteAsync(query, new { Id = id, UserId = userId });
            return rowsAffected > 0;
        }
    }
}
