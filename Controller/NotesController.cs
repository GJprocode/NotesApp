using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesBE.Data;
using NotesBE.Models;
using System.Security.Claims;

namespace NotesBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly NoteRepository _noteRepository;

        public NotesController(NoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        // Helper method to get UserId from claims
        private int? GetUserIdFromClaims()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotes([FromQuery] string? titleFilter = null)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();

            var notes = await _noteRepository.GetNotesByUserIdAsync(userId.Value);

            if (!string.IsNullOrEmpty(titleFilter))
            {
                notes = notes.Where(n => n.Title.Contains(titleFilter, StringComparison.OrdinalIgnoreCase));
            }

            return Ok(notes);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetNoteById(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();

            var note = await _noteRepository.GetNoteByIdAsync(id);
            if (note == null || note.UserId != userId)
            {
                return NotFound("Note not found or you do not have access.");
            }

            return Ok(note);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateNote([FromBody] Note note)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();

            note.UserId = userId.Value;
            note.CreatedAt = note.UpdatedAt = DateTime.UtcNow;

            try
            {
                var id = await _noteRepository.CreateNoteAsync(note);
                note.Id = id;
                return CreatedAtAction(nameof(GetNoteById), new { id = note.Id }, note);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] Note note)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();

            if (id != note.Id)
            {
                return BadRequest("Note ID mismatch.");
            }

            var existingNote = await _noteRepository.GetNoteByIdAsync(id);
            if (existingNote == null || existingNote.UserId != userId)
            {
                return NotFound("Note not found or you do not have access.");
            }

            note.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _noteRepository.UpdateNoteAsync(note);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();

            var existingNote = await _noteRepository.GetNoteByIdAsync(id);
            if (existingNote == null || existingNote.UserId != userId)
            {
                return NotFound("Note not found or you do not have access.");
            }

            try
            {
                await _noteRepository.DeleteNoteAsync(id, userId.Value);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
