using Dapper;
using System.Data;
using NotesBE.Models;

namespace NotesBE.Data
{
    public class NoteRepository
    {
        private readonly IDbConnection _dbConnection;

        public NoteRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Retrieves all notes for a specific user
        public async Task<IEnumerable<Note>> GetNotesByUserIdAsync(int userId)
        {
            const string query = "SELECT * FROM Notes WHERE UserId = @UserId";
            return await _dbConnection.QueryAsync<Note>(query, new { UserId = userId });
        }

        // Retrieves a note by its ID
        public async Task<Note?> GetNoteByIdAsync(int id)
        {
            const string query = "SELECT * FROM Notes WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Note>(query, new { Id = id });
        }

        // Creates a new note in the database
        public async Task<int> CreateNoteAsync(Note note)
        {
            const string query = @"
                INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
                VALUES (@UserId, @Title, @Content, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await _dbConnection.ExecuteScalarAsync<int>(query, note);
        }

        // Updates an existing note in the database
        public async Task<bool> UpdateNoteAsync(Note note)
        {
            const string query = @"
                UPDATE Notes
                SET Title = @Title, Content = @Content, UpdatedAt = @UpdatedAt
                WHERE Id = @Id AND UserId = @UserId;
            ";
            int rowsAffected = await _dbConnection.ExecuteAsync(query, note);
            return rowsAffected > 0;
        }

        // Deletes a note by ID and UserId
        public async Task<bool> DeleteNoteAsync(int id, int userId)
        {
            const string query = "DELETE FROM Notes WHERE Id = @Id AND UserId = @UserId";
            int rowsAffected = await _dbConnection.ExecuteAsync(query, new { Id = id, UserId = userId });
            return rowsAffected > 0;
        }
    }
}
