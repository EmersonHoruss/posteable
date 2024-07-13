using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Postable.Entities;
using Postable.Entities.Dtos;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Postable.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts(
            [FromQuery] string? username,
            [FromQuery] string orderBy = "createdAt",
            [FromQuery] string order = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var postsQuery = _context.Posts.Include(p => p.User).Include(p => p.Likes).AsQueryable();

            if (!string.IsNullOrEmpty(username))
            {
                postsQuery = postsQuery.Where(p => p.User.Username == username);
            }

            switch (orderBy.ToLower())
            {
                case "likescount":
                    postsQuery = order.ToLower() == "desc" ? postsQuery.OrderByDescending(p => p.Likes.Count) : postsQuery.OrderBy(p => p.Likes.Count);
                    break;
                default:
                    postsQuery = order.ToLower() == "desc" ? postsQuery.OrderByDescending(p => p.CreatedAt) : postsQuery.OrderBy(p => p.CreatedAt);
                    break;
            }

            var totalItems = await postsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var posts = await postsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostShowDto
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    UserName = p.User.Username,
                    LikesCount = p.Likes.Count
                })
                .ToListAsync();

            var response = new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                Items = posts
            };

            return Ok(response);
        }

        
    }
}
