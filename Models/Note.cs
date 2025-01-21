namespace NotesAppBackend.Models;

public class Note
{
    public int Id { get; set; }
    public int UserId { get; set; } // Reference to User
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
