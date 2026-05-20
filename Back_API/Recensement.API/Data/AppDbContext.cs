using Microsoft.EntityFrameworkCore;
using Recensement.API.Models; // Soloy amin'ny namespace marina anao raha ilaina

namespace Recensement.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Ireo tabilao ao amin'ny Database
        public DbSet<User> Users { get; set; }
        public DbSet<Menage> Menages { get; set; }
        public DbSet<Citoyen> Citoyens { get; set; }
        public DbSet<Rapport> Rapports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Eto isika no afaka manao "Configuration" fanampiny (ohatra: Primary Keys)
        }
    }
}