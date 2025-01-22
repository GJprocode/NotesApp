namespace NotesBE.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Input-only (not stored in DB)
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>(); // For VARBINARY(MAX)
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>(); // For VARBINARY(MAX)
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
