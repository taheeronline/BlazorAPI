using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlazorAPI.API.Models;

namespace BlazorAPI.API.Persistence.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable("Movies");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Title).IsRequired().HasMaxLength(255);
            builder.Property(m => m.Director).IsRequired().HasMaxLength(150);
            builder.Property(m => m.Genre).IsRequired().HasMaxLength(100);
            builder.Property(m => m.ReleaseDate).IsRequired();
            builder.Property(m => m.Rating).IsRequired();

            builder.Property(m => m.CreatedDate).IsRequired();
            builder.Property(m => m.ModifiedDate);

            builder.Property(m => m.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.HasQueryFilter(m => !m.IsDeleted?? false);

            builder.HasIndex(m => m.Title).HasDatabaseName("IX_Movies_Title");
            builder.HasIndex(m => m.Director).HasDatabaseName("IX_Movies_Director");
            builder.HasIndex(m => m.Genre).HasDatabaseName("IX_Movies_Genre");

            // 1. CreatedBy Relationship
            builder.HasOne(m => m.CreatedByUser)
                   .WithMany()
                   .HasForeignKey(m => m.CreatedBy)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);

            // 2. ModifiedBy Relationship
            builder.HasOne(m => m.ModifiedByUser)
                   .WithMany()
                   .HasForeignKey(m => m.ModifiedBy)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}