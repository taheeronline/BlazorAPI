namespace MovieManager.API.Models
{
    public class AuditLog : EntityBase
    {
        public Guid UserId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? ChangeDetails { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}
