namespace Postable.Entities.Dtos
{
    public class UserSignupDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
    }
}