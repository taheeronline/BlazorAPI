using Microsoft.EntityFrameworkCore;
using MovieManager.API.Models;
using System.Reflection;

namespace MovieManager.API.Persistence
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; } 
        public DbSet<Book> Books { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            //modelBuilder.ApplyConfiguration(new MovieConfiguration());
            //modelBuilder.ApplyConfiguration(new UserConfiguration());

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
    }
}
