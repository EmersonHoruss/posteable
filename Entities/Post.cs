namespace Postable.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;
    }
}