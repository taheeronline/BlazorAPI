using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieManager.API.Models;

namespace MovieManager.API.Persistence.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
            builder.Property(b => b.Description).HasMaxLength(500);
            builder.Property(b => b.Author).IsRequired().HasMaxLength(100);
            builder.Property(b => b.Publisher).IsRequired().HasMaxLength(100);
            builder.Property(b => b.Price).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(b => b.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.HasQueryFilter(b => !b.IsDeleted ?? false);

            builder.HasIndex(b => b.Title).HasDatabaseName("IX_Books_Title");
            builder.HasIndex(b => b.Author).HasDatabaseName("IX_Books_Author");
            builder.HasIndex(b => b.Publisher).HasDatabaseName("IX_Books_Publisher");

            // 1. CreatedBy Relationship
            builder.HasOne(b => b.CreatedByUser)
                   .WithMany()
                   .HasForeignKey(b => b.CreatedBy)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);

            // 2. ModifiedBy Relationship
            builder.HasOne(b => b.ModifiedByUser)
                   .WithMany()
                   .HasForeignKey(b => b.ModifiedBy)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
