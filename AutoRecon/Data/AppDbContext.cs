using AutoRecon.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRecon.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Target> Targets { get; set; }
        public DbSet<Scan> Scans { get; set; }
        public DbSet<Vulnerability> Vulnerabilities { get; set; }
    }
}
