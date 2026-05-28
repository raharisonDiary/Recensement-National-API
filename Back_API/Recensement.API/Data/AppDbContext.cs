using Microsoft.EntityFrameworkCore;
using Recensement.API.Models;

namespace Recensement.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Ireo tabilao ao amin'ny Database
        public DbSet<User> Users { get; set; }
        public DbSet<AgentProfile> AgentProfiles { get; set; } // Nampiana ity
        public DbSet<Menage> Menages { get; set; }
        public DbSet<Citoyen> Citoyens { get; set; }
        public DbSet<Rapport> Rapports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration ny fifandraisana (One-to-One: User <-> AgentProfile)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<AgentProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Raha fafana ny User, fafana koa ny Profile
            
            // Configuration hafa (ohatra: Unique index ho an'ny CIN)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Cin)
                .IsUnique();
        }
    }
}