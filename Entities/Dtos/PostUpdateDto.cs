using System.ComponentModel.DataAnnotations;

namespace Postable.Entities.Dtos
{
    public class PostUpdateDto
    {
        [MaxLength(500)]
        public string? Content { get; set; }
    }
}
