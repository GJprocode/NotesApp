namespace NotesBE.Models;

// Represents a single note created by a user.
public class Note
{
    public int Id { get; set; } // Unique identifier for the note.
    public int UserId { get; set; } // ID of the user who created the note.
    public string Title { get; set; } = null!; // The title of the note (required).
    public string? Content { get; set; } // Optional content of the note.
    public DateTime CreatedAt { get; set; } // Timestamp for when the note was created.
    public DateTime UpdatedAt { get; set; } // Timestamp for when the note was last updated.
}
