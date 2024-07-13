using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Postable.Entities;
using Postable.Entities.Dtos;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace Postable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<ActionResult> GetUser()
        {
            try
            {
                var userName = User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                if (string.IsNullOrEmpty(userName))
                {
                    return Unauthorized(new
                    {
                        code = 401,
                        error = "Unauthorized",
                        message = "User is not authenticated.",
                        timestamp = DateTime.UtcNow
                    });
                }

                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userName);
                if (user == null)
                {
                    return NotFound(new
                    {
                        code = 404,
                        error = "Not Found",
                        message = "User not found.",
                        timestamp = DateTime.UtcNow
                    });
                }

                return Ok(new UserShowDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
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

        [HttpPatch("me")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto userUpdateDto)
        {
            try
            {
                var userName = User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                if (string.IsNullOrEmpty(userName))
                {
                    return Unauthorized(new
                    {
                        code = 401,
                        error = "Unauthorized",
                        message = "User is not authenticated.",
                        timestamp = DateTime.UtcNow
                    });
                }

                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userName);
                if (user == null)
                {
                    return NotFound(new
                    {
                        code = 404,
                        error = "Not Found",
                        message = "User not found.",
                        timestamp = DateTime.UtcNow
                    });
                }

                user.Username = userUpdateDto.Username ?? user.Username;
                user.Password = userUpdateDto.Password ?? user.Password;
                user.Email = userUpdateDto.Email ?? user.Email;
                user.FirstName = userUpdateDto.FirstName ?? user.FirstName;
                user.LastName = userUpdateDto.LastName ?? user.LastName;
                user.Role = userUpdateDto.Role ?? user.Role;

                // Just two roles are valid, user and admin
            if (userUpdateDto.Role != null &&
                    !string.Equals(userUpdateDto.Role, "User", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(userUpdateDto.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new
                    {
                        code = 400,
                        error = "Bad Request",
                        message = "Invalid role. Only 'User' and 'Admin' are valid roles.",
                        timestamp = DateTime.UtcNow
                    });
                }

                // Save role in lower case
                if (userUpdateDto.Role != null)
                {
                    user.Role = userUpdateDto.Role.ToLower();
                }
                await _context.SaveChangesAsync();

                return Ok(new UserShowDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                });
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

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteUser()
        {
            try
            {
                var userName = User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                if (string.IsNullOrEmpty(userName))
                {
                    return Unauthorized(new
                    {
                        code = 401,
                        error = "Unauthorized",
                        message = "User is not authenticated.",
                        timestamp = DateTime.UtcNow
                    });
                }

                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userName);
                if (user == null)
                {
                    return NotFound(new
                    {
                        code = 404,
                        error = "Not Found",
                        message = "User not found.",
                        timestamp = DateTime.UtcNow
                    });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return NoContent();
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
    }
}
