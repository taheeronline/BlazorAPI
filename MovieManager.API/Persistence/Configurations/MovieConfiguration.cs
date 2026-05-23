using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieManager.API.Models;

namespace MovieManager.API.Persistence.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            // Table configuration
            builder.ToTable("Movies");

            // Primary key
            builder.HasKey(m => m.Id);

            // Property configurations
            builder.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(255)
                .HasComment("Movie title");

            builder.Property(m => m.Director)
                .IsRequired()
                .HasMaxLength(150)
                .HasComment("Movie director name");

            builder.Property(m => m.Genre)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("Movie genre category");

            builder.Property(m => m.ReleaseDate)
                .IsRequired()
                .HasComment("Movie release date");

            builder.Property(m => m.Rating)
                .IsRequired()
                .HasComment("Movie rating (0-10)");

            builder.Property(m => m.Created)
                .IsRequired()
                .HasComment("Record creation timestamp");

            builder.Property(m => m.LastModified)
                .IsRequired()
                .HasComment("Record last modification timestamp");

            // Indexes for search performance
            builder.HasIndex(m => m.Title)
                .HasDatabaseName("IX_Movies_Title");

            builder.HasIndex(m => m.Director)
                .HasDatabaseName("IX_Movies_Director");

            builder.HasIndex(m => m.Genre)
                .HasDatabaseName("IX_Movies_Genre");
        }
    }
}
