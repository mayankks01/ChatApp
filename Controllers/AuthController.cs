using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChatApp.DTOs.UserDTOs;
using ChatApp.Data;
using Microsoft.EntityFrameworkCore;
using ChatApp.Models;
using ChatApp.Services;
using AutoMapper;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ChatAppDbContext _dbContext;
        private readonly JwtService _jwtService;
        private readonly IMapper _mapper;
        public AuthController(ChatAppDbContext dbContext, JwtService jwtService, IMapper mapper)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
            _mapper = mapper;
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

            var existingRole = await _dbContext.Roles.FirstOrDefaultAsync(role => role.RoleName == dto.Role);

            if(existingRole == null)
            {
                var roleToCreate = new Role
                {
                  RoleName= dto.Role
                };
                await _dbContext.Roles.AddAsync(roleToCreate);
                await _dbContext.SaveChangesAsync();
                existingRole = roleToCreate;
            }

            var user = new User
            {
                UserName = dto.UserName,
                UserEmail = dto.UserEmail,
                Password = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        RoleId = existingRole.RoleId
                    }
                }
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            //jwt Token->
            var token = _jwtService.GenerateToken(dto);

            var userToSend = _mapper.Map<RegisterResponseDTO>(user);

            return Ok(new { token , user=userToSend });
        }
    }
}
