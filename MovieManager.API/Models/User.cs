using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieManager.API.Models
{
    public class User : EntityBase
    {
        [Required]
        [MaxLength(450)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(450)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";

        public DateTimeOffset? LastLogin { get; set; }

        public ICollection<LoginLog>? LoginLogs { get; set; }
        public ICollection<AuditLog>? AuditLogs { get; set; }
    }
}
