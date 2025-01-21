using Microsoft.AspNetCore.Mvc;
using NotesAppBackend.Data;
using NotesAppBackend.Models;
using NotesAppBackend.Utils;
using System;

namespace NotesAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly string _jwtSecret;

        public AuthController(UserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _jwtSecret = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT secret is not configured.");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(user.Password))
                return BadRequest("Password is required.");

            // Generate password hash and salt
            PasswordHelper.CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.CreatedAt = user.UpdatedAt = DateTime.UtcNow;

            var userId = await _userRepository.CreateUserAsync(user);
            return Ok(new { Message = "User registered successfully", UserId = userId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var existingUser = await _userRepository.GetUserByUsernameAsync(user.Username);
            if (existingUser == null || existingUser.PasswordHash == null || existingUser.PasswordSalt == null)
                return Unauthorized("Invalid username or password.");

            // Verify password hash
            if (!PasswordHelper.VerifyPasswordHash(user.Password, existingUser.PasswordHash, existingUser.PasswordSalt))
                return Unauthorized("Invalid username or password.");

            // Generate JWT token
            var token = JwtHelper.GenerateToken(existingUser.Username, existingUser.Id, _jwtSecret);
            return Ok(new { Token = token });
        }
    }
}
