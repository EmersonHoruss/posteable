namespace Postable.Entities.Dtos
{
    public class PostShowDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        public int LikesCount { get; set; }
    }
}
