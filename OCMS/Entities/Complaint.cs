using OCMS.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace OCMS.Entities
{
    public class Complaint
    {
        [Key]
        public Guid ComplaintId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? ImagePath { get; set; }
        public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;
        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;
        public string TrackId { get; set; } = string.Empty;   // C-1234 like ID
        // ================== NAVIGATION PROPERTIES ==================
        public User User { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ICollection<ComplaintResponse> Responses { get; set; } = new List<ComplaintResponse>();

    }
}
