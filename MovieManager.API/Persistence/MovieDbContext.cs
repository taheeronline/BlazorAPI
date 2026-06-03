using Microsoft.EntityFrameworkCore;
using MovieManager.API.Models;
using MovieManager.API.Persistence.Configurations;

namespace MovieManager.API.Persistence
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LoginLog> LoginLogs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.ApplyConfiguration(new MovieConfiguration());

            // Configure relationships
            modelBuilder.Entity<User>(b =>
            {
                b.HasMany(u => u.LoginLogs).WithOne(ll => ll.User).HasForeignKey(ll => ll.UserId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(u => u.AuditLogs).WithOne(al => al.User).HasForeignKey(al => al.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<LoginLog>(b =>
            {
                b.Property(p => p.IpAddress).HasColumnType("nvarchar(max)");
                b.Property(p => p.UserAgent).HasColumnType("nvarchar(max)");
            });

            modelBuilder.Entity<AuditLog>(b =>
            {
                b.Property(p => p.EntityType).HasColumnType("nvarchar(max)");
                b.Property(p => p.Action).HasColumnType("nvarchar(max)");
                b.Property(p => p.ChangeDetails).HasColumnType("nvarchar(max)");
            });
        }
    }
}
