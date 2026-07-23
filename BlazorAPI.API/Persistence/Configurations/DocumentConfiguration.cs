using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BlazorAPI.API.Models;

namespace BlazorAPI.API.Persistence.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
            builder.Property(d => d.FileName).IsRequired().HasMaxLength(255);
            builder.Property(d => d.ContentType).IsRequired().HasMaxLength(2000000);
            builder.Property(d => d.FileSize).IsRequired();
            builder.Property(d => d.FileContent).IsRequired(); // EF automatically maps byte[] to varbinary(max)

            builder.Property(d => d.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.HasQueryFilter(d => !d.IsDeleted ?? false);

            builder.HasIndex(d => d.Name).HasDatabaseName("IX_Documents_Name");
            builder.HasIndex(d => d.FileName).HasDatabaseName("IX_Documents_FileName");
            builder.HasIndex(d => d.ContentType).HasDatabaseName("IX_Documents_ContentType");

            // 1. CreatedBy Relationship
            builder.HasOne(d => d.CreatedByUser)
                   .WithMany()
                   .HasForeignKey(d => d.CreatedBy)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);

            // 2. ModifiedBy Relationship
            builder.HasOne(d => d.ModifiedByUser)
                   .WithMany()
                   .HasForeignKey(d => d.ModifiedBy)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}