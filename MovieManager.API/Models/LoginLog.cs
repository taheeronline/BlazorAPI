namespace MovieManager.API.Models
{
    public class LoginLog : EntityBase
    {
        public Guid UserId { get; set; }
        public DateTimeOffset LoginTime { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? LogoutTime { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}
