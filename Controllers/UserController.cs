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
        public async Task<ActionResult> GetMyProfile()
        {
            var userName = User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userName);
            if (user == null)
            {
                return Unauthorized();
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

        [HttpPatch("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UserUpdateDto userUpdateDto)
        {
            var userName = User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userName);
            if (user == null)
            {
                return Unauthorized();
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
                return BadRequest("Invalid role, Just user and admin are valid roles.");
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
    }
}
