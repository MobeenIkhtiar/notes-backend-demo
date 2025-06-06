using Microsoft.EntityFrameworkCore;

namespace demo_docker.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> op):base(op) { }

        public DbSet<Notes> Notes { get; set; }
    }

}
