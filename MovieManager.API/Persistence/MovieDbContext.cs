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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.ApplyConfiguration(new MovieConfiguration());
        }
    }
}
