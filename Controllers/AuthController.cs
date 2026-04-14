using System.Security.Claims;
using HRManagement.Data;
using HRManagement.DTOs.UserDTOs;
using HRManagement.Models;
using HRManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HRDbContext _db;
        private readonly JwtService _jwt;

        private readonly RefreshTokenGeneratorService _refreshToken;
        public AuthController(HRDbContext db, JwtService jwt, RefreshTokenGeneratorService refreshToken)
        {
            _db = db;
            _jwt = jwt;
            _refreshToken = refreshToken;
        }

        [HttpGet("roles")]
        public async Task<ActionResult> Roles()
        {

            List<RoleDTO> Roles = await _db.Roles.Select(r => new RoleDTO
            {
                Code = r.RoleId,
                CodeDesc = r.RoleName
            }).ToListAsync();

            return Ok(Roles);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginUserDTO userDTO)
        {
            if (string.IsNullOrWhiteSpace(userDTO.Email))
                return BadRequest(new { Message = "Please enter valid email" });
            if (string.IsNullOrEmpty(userDTO.Password))
                return BadRequest(new { Message = "Password cannot be empty" });
            var User = await _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == userDTO.Email);
            if (User == null)
            {
                return BadRequest(new { Message = "User doesnot exist" });
            }
            var isPasswordMatches = BCrypt.Net.BCrypt.Verify(userDTO.Password, User.PasswordHash);
            if (!isPasswordMatches)
            {
                return BadRequest(new { Message = "Please enter correct Password" });
            }
            var roles = User.UserRoles.Select(ur => ur.Role.RoleName).ToList();
            var jwtObj = new JwtDTO
            {
                Email = userDTO.Email,
                Roles = roles
            };
            var ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            var token = _jwt.GenerateToken(jwtObj);
            var (refreshToken, rawToken) = _refreshToken.CreateRefreshToken(User.UserId, ipAddress);
            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();
            return Ok(new { AccessToken = token, RefreshToken = rawToken, roles = roles });
        }

        [HttpPost("logout")]
        public async Task<ActionResult> logout([FromBody] LogoutDTO logoutObj)
        {
            var hashedIncomingToken = _refreshToken.ComputeSha256Hash(logoutObj.refreshToken);
            var tokenFromDb = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == hashedIncomingToken);
            tokenFromDb.IsRevoked = true;
            await _db.SaveChangesAsync();
            return Ok(new {Message="User logged out successfully"});
        }


        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegistrationUser userDTO)
        {
            Console.WriteLine("I am here got hit by register route ");
            if (userDTO == null)
            {
                return BadRequest(new { Message = "User cannot be empty" });
            }
            if (string.IsNullOrWhiteSpace(userDTO.Email))
            {
                return BadRequest(new { Message = "Invalid Email" });
            }
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == userDTO.Email);
            if (existingUser != null)
            {
                return BadRequest(new { Message = "User with Email already exists" });
            }
            var rolesFromDB = await _db.Roles.Where(r => userDTO.Roles.Contains(r.RoleName)).ToListAsync();
            if (rolesFromDB.Count != userDTO.Roles.Count)
            {
                return BadRequest(new { Message = "Please select frome existing roles" });
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
                UserRoles = rolesFromDB.Select(role => new UserRole { RoleId = role.RoleId }).ToList()
            };

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "Registration Successful, Please Login" });
        }


        [Authorize(Roles = "HR")]
        [HttpGet("employees")]
        public string GetEmployees()
        {
            return "Employees";
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("hr")]
        public string GetHRs()
        {
            return "khkhkh";
        }

        [Authorize]
        [HttpPost("refreshtoken")]
        public async Task<ActionResult> RefreshToken([FromBody] jwtRefreshTokenDTO jwtObj)
        {
            var hashedIncomingToken = _refreshToken.ComputeSha256Hash(jwtObj.RefreshToken);
            var tokenFromDb = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == hashedIncomingToken);
            if (tokenFromDb == null)
            {
                return Unauthorized("Invalid refresh token");
            }
            if (tokenFromDb.IsRevoked)
            {
                return Unauthorized("Token Expired");
            }
            if (tokenFromDb.Expires < DateTime.UtcNow)
            {
                return Unauthorized("Token Expired");
            }
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var roles = User.FindAll(ClaimTypes.Role)
                            .Select(r => r.Value).ToList();
            var jwtObjClaims = new JwtDTO
            {
                Email = email,
                Roles = roles
            };
            var ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            var token = _jwt.GenerateToken(jwtObjClaims);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == jwtObjClaims.Email);
            var (newRefreshToken, rawToken) = _refreshToken.CreateRefreshToken(user.UserId, ipAddress);
            tokenFromDb.IsRevoked = true;
            await _db.RefreshTokens.AddAsync(newRefreshToken);
            await _db.SaveChangesAsync();
            return Ok(new { accessToken = token, refreshToken = rawToken });
        }
    }
}
