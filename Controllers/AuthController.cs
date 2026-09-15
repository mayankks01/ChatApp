using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChatApp.DTOs.UserDTOs;
using ChatApp.Data;
using Microsoft.EntityFrameworkCore;
using ChatApp.Models;
using ChatApp.Services;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ChatAppDbContext _dbContext;
        private readonly JwtService _jwtService;
        public AuthController(ChatAppDbContext dbContext, JwtService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        [HttpPost("Login")]
        public ActionResult Login()
        {
            return Ok("Login successful");
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequestDTO dto)
        {
            if(dto== null)
            {
                return BadRequest("Invalid request");
            }

            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.UserEmail == dto.UserEmail);

            if(existingUser != null)
            {
                return BadRequest("User already exists");
            }

            var hashedPassword=BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                UserName = dto.UserName,
                UserEmail = dto.UserEmail,
                Password = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            //jwt Token->
            var token = _jwtService.GenerateToken(dto);


            return Ok(new { token });
        }
    }
}
