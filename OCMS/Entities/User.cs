using OCMS.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace OCMS.Entities
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserStatus Status { get; set; } = UserStatus.Active;
        public string? ImageLink { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }
        // Navigation
        public UserCredential? Credential { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = [];
        public ICollection<Complaint> Complaints { get; set; } = [];
    }
}
