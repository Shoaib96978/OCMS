using Microsoft.EntityFrameworkCore;
using OCMS.Entities;

namespace OCMS.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserCredential> UserCredentials { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}
