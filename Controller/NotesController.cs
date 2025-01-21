using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesAppBackend.Data;
using NotesAppBackend.Models;

namespace NotesAppBackend.Controllers
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotes([FromQuery] string? titleFilter, [FromQuery] int? userIdFilter)
        {
            var notes = await _noteRepository.GetAllNotesAsync();

            if (!string.IsNullOrEmpty(titleFilter))
                notes = notes.Where(n => n.Title.Contains(titleFilter, StringComparison.OrdinalIgnoreCase));

            if (userIdFilter.HasValue)
                notes = notes.Where(n => n.UserId == userIdFilter);

            return Ok(notes);
        }

        // Search Notes endpoint
        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchNotes([FromQuery] string? query)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest("Search query cannot be empty.");

            var notes = await _noteRepository.GetAllNotesAsync();

            // Search in Title and Content
            var filteredNotes = notes.Where(n =>
                (!string.IsNullOrEmpty(n.Title) && n.Title.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(n.Content) && n.Content.Contains(query, StringComparison.OrdinalIgnoreCase))
            );

            return Ok(filteredNotes);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetNoteById(int id)
        {
            var note = await _noteRepository.GetNoteByIdAsync(id);
            if (note == null)
                return NotFound();
            return Ok(note);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateNote([FromBody] Note note)
        {
            note.CreatedAt = note.UpdatedAt = DateTime.UtcNow;
            var id = await _noteRepository.CreateNoteAsync(note);
            note.Id = id;
            return CreatedAtAction(nameof(GetNoteById), new { id = note.Id }, note);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] Note note)
        {
            if (id != note.Id)
                return BadRequest();
            var existingNote = await _noteRepository.GetNoteByIdAsync(id);
            if (existingNote == null)
                return NotFound();
            note.UpdatedAt = DateTime.UtcNow;
            await _noteRepository.UpdateNoteAsync(note);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var existingNote = await _noteRepository.GetNoteByIdAsync(id);
            if (existingNote == null)
                return NotFound();
            await _noteRepository.DeleteNoteAsync(id, existingNote.UserId);
            return NoContent();
        }
    }
}
