using System.ComponentModel.DataAnnotations;

namespace Postable.Entities.Dtos
{
    public class UserUpdateDto
    {
        [MinLength(1)]
        [MaxLength(100)]
        public string? Username { get; set; }
        
        [MinLength(1)]
        [MaxLength(100)]
        public string? Password { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [MinLength(1)]
        [MaxLength(20)]
        public string? Role { get; set; }
    }
}