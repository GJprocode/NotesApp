using Microsoft.AspNetCore.Authorization; // For securing endpoints with authorization policies.
using Microsoft.AspNetCore.Mvc; // Provides functionality for API controller actions.
using NotesBE.Data; // Repository for interacting with the Notes table.
using NotesBE.Models; // Contains the Note model.

namespace NotesBE.Controllers
{
    [ApiController] // Indicates that this class is an API controller.
    [Route("api/[controller]")] // Defines the base route for this controller as "api/notes".
    public class NotesController : ControllerBase
    {
        private readonly NoteRepository _noteRepository; // Repository for managing note operations.

        public NotesController(NoteRepository noteRepository)
        {
            _noteRepository = noteRepository; // Dependency injection of the NoteRepository.
        }

        // GET: api/notes
        [HttpGet]
        [Authorize] // Ensures the endpoint is accessible only by authenticated users.
        public async Task<IActionResult> GetNotes([FromQuery] string? titleFilter = null)
        {
            // Retrieve the user ID from the JWT claims.
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null) return Unauthorized(); // User is not authorized if the claim is missing.

            int userId = int.Parse(userIdClaim.Value); // Parse the userId from the claim.
            var notes = await _noteRepository.GetNotesByUserIdAsync(userId); // Fetch notes for the user.

            // Apply title filter if provided.
            if (!string.IsNullOrEmpty(titleFilter))
            {
                notes = notes.Where(n => n.Title.Contains(titleFilter, StringComparison.OrdinalIgnoreCase));
            }

            return Ok(notes); // Return the filtered notes.
        }

        // POST: api/notes
        [HttpPost]
        [Authorize] // Ensures only authenticated users can create notes.
        public async Task<IActionResult> CreateNote([FromBody] Note note)
        {
            // Retrieve the user ID from the JWT claims.
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value); // Parse the userId from the claim.
            note.UserId = userId; // Assign the userId to the note.
            note.CreatedAt = note.UpdatedAt = DateTime.UtcNow; // Set timestamps for note creation.

            var id = await _noteRepository.CreateNoteAsync(note); // Save the note to the database.
            note.Id = id; // Assign the generated ID to the note object.

            return CreatedAtAction(nameof(GetNotes), new { id = note.Id }, note); // Return the created note.
        }

        // PUT: api/notes/{id}
        [HttpPut("{id}")]
        [Authorize] // Ensures only authenticated users can update notes.
        public async Task<IActionResult> UpdateNote(int id, [FromBody] Note note)
        {
            if (id != note.Id) return BadRequest(); // Validate that the path ID matches the note ID.

            // Retrieve the user ID from the JWT claims.
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value); // Parse the userId from the claim.

            // Check if the note exists and belongs to the current user.
            var existingNote = await _noteRepository.GetNoteByIdAsync(id);
            if (existingNote == null || existingNote.UserId != userId) return NotFound();

            note.UpdatedAt = DateTime.UtcNow; // Update the timestamp.
            await _noteRepository.UpdateNoteAsync(note); // Save the changes to the database.

            return NoContent(); // Return a success response with no content.
        }

        // DELETE: api/notes/{id}
        [HttpDelete("{id}")]
        [Authorize] // Ensures only authenticated users can delete notes.
        public async Task<IActionResult> DeleteNote(int id)
        {
            // Retrieve the user ID from the JWT claims.
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value); // Parse the userId from the claim.

            // Check if the note exists and belongs to the current user.
            var existingNote = await _noteRepository.GetNoteByIdAsync(id);
            if (existingNote == null || existingNote.UserId != userId) return NotFound();

            await _noteRepository.DeleteNoteAsync(id, userId); // Delete the note.
            return NoContent(); // Return a success response with no content.
        }
    }
}
