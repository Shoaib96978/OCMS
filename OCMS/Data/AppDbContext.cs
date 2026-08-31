using Microsoft.EntityFrameworkCore;
using OCMS.Entities;

namespace OCMS.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
