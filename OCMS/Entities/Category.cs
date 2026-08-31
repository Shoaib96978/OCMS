using System.ComponentModel.DataAnnotations;

namespace OCMS.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        // Navigation (optional but useful)
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}
