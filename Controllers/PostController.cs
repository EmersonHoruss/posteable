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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] PostCreateDto postCreateDto)
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

            var post = new Post { UserId = user.Id, Content = postCreateDto.Content };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            var postShowDto = new PostShowDto
            {
                Id = post.Id,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UserName = user.Username,
                LikesCount = post.Likes.Count
            };

            return CreatedAtAction(nameof(GetPosts), new { id = post.Id }, postShowDto);
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] PostUpdateDto postUpdateDto)
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

            var post = await _context.Posts.Include(p => p.User).SingleOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            post.Content = postUpdateDto.Content ?? post.Content;
            await _context.SaveChangesAsync();

            var postShowDto = new PostShowDto
            {
                Id = post.Id,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UserName = post.User.Username,
                LikesCount = post.Likes.Count
            };

            return Ok(postShowDto);
        }

        [HttpPost("{postId}/like")]
        [Authorize]
        public async Task<IActionResult> LikePost(int postId)
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

            var post = await _context.Posts.Include(p => p.Likes).SingleOrDefaultAsync(p => p.Id == postId);
            if (user == null || post == null)
            {
                return NotFound();
            }

            if (post.Likes.Any(l => l.UserId == user.Id))
            {
                return BadRequest("Post already liked by user.");
            }

            var like = new Like { PostId = postId, UserId = user.Id, CreatedAt = DateTime.UtcNow };
            post.Likes.Add(like);

            await _context.SaveChangesAsync();

            var postShowDto = new PostShowDto
            {
                Id = post.Id,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UserName = post.User.Username,
                LikesCount = post.Likes.Count
            };

            return Ok(postShowDto);
        }

        [HttpDelete("{postId}/like")]
        [Authorize]
        public async Task<IActionResult> UnlikePost(int postId)
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

            var post = await _context.Posts.Include(p => p.Likes).SingleOrDefaultAsync(p => p.Id == postId);
            var like = post.Likes.SingleOrDefault(l => l.UserId == user.Id);
            if (post == null || like == null)
            {
                return NotFound();
            }

            post.Likes.Remove(like);
            await _context.SaveChangesAsync();

            var postShowDto = new PostShowDto
            {
                Id = post.Id,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UserName = post.User.Username,
                LikesCount = post.Likes.Count
            };

            return Ok(postShowDto);
        }
    }
}
