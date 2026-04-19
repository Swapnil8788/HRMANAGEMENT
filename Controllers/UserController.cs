using HRManagement.Data;
using HRManagement.DTOs.UserDTOs;
using HRManagement.Enums;
using HRManagement.Migrations;
using HRManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly HRDbContext _db;
        public UserController(HRDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users.ToListAsync();
            return Ok(new { data = users });
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUser userDTO)
        {
            if (userDTO == null)
            {
                return BadRequest(new { Message = "Invalid User" });
            }
            var userFromDB = await _db.Users.FirstOrDefaultAsync(u => u.Email == userDTO.Email);
            if (userFromDB != null)
            {
                return BadRequest(new { Message = "User already exists" });
            }
            var hashedPasswrod = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
            var user = new User
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                PhoneNo = userDTO.PhoneNo,
                PasswordHash = hashedPasswrod,
                Age = userDTO.Age,
                UserStatus = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow

            };
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "User Created Successfully" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] CreateUser userDTO)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }
            if (!string.IsNullOrEmpty(userDTO.Password))
            {
                var hashedPasswrod = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
                user.PasswordHash = hashedPasswrod;
            }

            user.Name = userDTO.Name;
            user.Email = userDTO.Email;
            user.PhoneNo = userDTO.PhoneNo;
            user.Age = userDTO.Age;
            // user.UserRoles = userDTO.Roles;
            // user.UserStatus = userDTO.
            user.ModifiedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new { Message = "User updated successfully", password = userDTO.Password });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }
            var userDTO = new UserDTO
            {
                Id = user.UserId,
                Name = user.Name,
                Email = user.Email,
                PhoneNo = user.PhoneNo,
                Age = user.Age
            };
            return Ok(new { data = userDTO });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "User Deleted Succussfully" });
        }
    }
}
