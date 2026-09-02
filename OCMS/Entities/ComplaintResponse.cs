using System.ComponentModel.DataAnnotations;

namespace OCMS.Entities
{
    public class ComplaintResponse
    {
        [Key]
        public Guid ResponseId { get; set; } = Guid.NewGuid();
        public Guid ComplaintId { get; set; }
        public Guid UserId { get; set; }               
        public string? Description { get; set; }
        public DateTime ResponseDate { get; set; } = DateTime.UtcNow;

        // ================== NAVIGATION PROPERTIES ==================
        public Complaint Complaint { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
