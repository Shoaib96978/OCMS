using Microsoft.EntityFrameworkCore;
using OCMS.Entities;

namespace OCMS.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserCredential> UserCredentials { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<ComplaintResponse> ComplaintResponses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                         new Category { CategoryId = 1, CategoryName = "Electricity" },
                         new Category { CategoryId = 2, CategoryName = "Water Supply" },
                         new Category { CategoryId = 3, CategoryName = "Road Damage" },
                         new Category { CategoryId = 4, CategoryName = "Billing Problems" },
                         new Category { CategoryId = 5, CategoryName = "Technical Support" },
                         new Category { CategoryId = 6, CategoryName = "Garbage Collection" }
                         );
        }

    }
}
