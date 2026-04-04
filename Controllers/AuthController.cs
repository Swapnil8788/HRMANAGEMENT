using HRManagement.Data;
using HRManagement.DTOs.UserDTOs;
using HRManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HRDbContext _db;
        public AuthController(HRDbContext db)
        {
            _db = db;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginUserDTO userDTO)
        {
            if (string.IsNullOrWhiteSpace(userDTO.Email))
                return BadRequest(new {Message = "Please enter valid email"});
            if(string.IsNullOrEmpty(userDTO.Password))
                return BadRequest(new {Message = "Password cannot be empty"});
            var User = await _db.Users.FirstOrDefaultAsync(u => u.Email == userDTO.Email);
            if(User == null)
            {
                return BadRequest(new {Message = "User doesnot exist"});
            }
            var isPasswordMatches = BCrypt.Net.BCrypt.Verify(userDTO.Password, User.PasswordHash);
            if (!isPasswordMatches)
            {
                return BadRequest(new {Message="Please enter correct Password"});
            }
            return Ok(new {Message = "Login Sucessful"});
        }  


        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegistrationUser userDTO)
        {
            Console.WriteLine("I am here got hit by register route ");
            if(userDTO == null)
            {
                return BadRequest(new {Message ="User cannot be empty"});
            }
            if (string.IsNullOrWhiteSpace(userDTO.Email))
            {
                return BadRequest(new {Message = "Invalid Email"});
            }
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == userDTO.Email);
            if(existingUser != null)
            {
                return BadRequest(new { Message="User with Email already exists"});
            }
            var rolesFromDB = await _db.Roles.Where(r => userDTO.Roles.Contains(r.RoleName)).ToListAsync();
            if(rolesFromDB.Count != userDTO.Roles.Count)
            {
                return BadRequest(new {Message = "Please select frome existing roles"});
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);

            var user = new User
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                PhoneNo = userDTO.PhoneNo,
                PasswordHash = hashedPassword,
                Age = userDTO.Age,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                UserRoles = rolesFromDB.Select(role => new UserRole{RoleId = role.RoleId}).ToList()
            };

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "Registration Successful, Please Login" });
        }
    }
}
