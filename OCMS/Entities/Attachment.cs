using System.ComponentModel.DataAnnotations;

namespace OCMS.Entities
{
    public class Attachment
    {
        [Key]
        public int AttachmentId { get; set; }
        public int ComplaintId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;   // wwwroot/uploads/...
        public long FileSize { get; set; }
        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    }
}
