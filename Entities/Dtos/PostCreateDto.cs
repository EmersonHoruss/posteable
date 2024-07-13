using System.ComponentModel.DataAnnotations;

namespace Postable.Entities.Dtos
{
    public class PostCreateDto
    {
        [Required]
        [MaxLength(500)]
        public string Content { get; set; }
    }
}
