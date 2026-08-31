using OCMS.Entities;
using System.ComponentModel.DataAnnotations;

namespace OCMS.Entities
{
    public class UserCredential
    {
        [Key]
        public Guid CredentialId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public byte[] PasswordHash { get; set; } = [];
        public byte[] PasswordSalt { get; set; } = [];
        public string? Otp { get; set; }
        public DateTime? OtpExpiry { get; set; }
        public User User { get; set; } = null!;
    }
}
