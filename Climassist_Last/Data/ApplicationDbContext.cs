using Microsoft.EntityFrameworkCore;
using Climassist_Last.Models;

namespace Climassist_Last.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Requests> Requests { get; set; }
    }
}