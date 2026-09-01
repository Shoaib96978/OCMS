using OCMS.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace OCMS.Entities
{
    public class UserRole
    {
        [Key]
        public Guid UserRoleId { get; set; } = Guid.NewGuid();
        public AppRoles Role { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
