using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Postable.Entities;
using Postable.Entities.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Postable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] UserCreateDto userCreateDto)
        {
            try
            {
                var user = new User
                {
                    Username = userCreateDto.Username,
                    Password = userCreateDto.Password,
                    Email = userCreateDto.Email,
                    FirstName = userCreateDto.FirstName,
                    LastName = userCreateDto.LastName,
                    Role = userCreateDto.Role ?? "user",
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userShowDto = new UserShowDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                };

                return Created(string.Empty, userShowDto);
            }
            catch (DbUpdateException ex)
            {
                if ((ex.InnerException as Microsoft.Data.SqlClient.SqlException)?.Number == 2601)
                {
                    return Conflict(new
                    {
                        code = 409,
                        error = "Conflict",
                        message = "Username or Email already exists.",
                        timestamp = DateTime.UtcNow
                    });
                }

                return StatusCode(500, new
                {
                    code = 500,
                    error = "Internal Server Error",
                    message = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = 500,
                    error = "Internal Server Error",
                    message = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto userLogin)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == userLogin.Username && u.Password == userLogin.Password);
            if (user == null)
            {
                return StatusCode(401, new
                {
                    code = 401,
                    error = "Unauthorized",
                    message = "Incorrect credentials.",
                    timestamp = DateTime.UtcNow
                });
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
