namespace Postable.Entities
{
    public class Like
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Post Post { get; set; }
        public User User { get; set; }
    }
}