using System.ComponentModel.DataAnnotations;

namespace Postable.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; } 
        public required string Password { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}