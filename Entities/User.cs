using System.ComponentModel.DataAnnotations;

namespace Postable.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; } = string.Empty;
        public required string Password { get; set; } = string.Empty;
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; } = "user";
        public DateTime? CreatedAt { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}