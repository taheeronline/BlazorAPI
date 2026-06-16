using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieManager.API.Models;

namespace MovieManager.API.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
            builder.Property(u => u.UserName).IsRequired().HasMaxLength(25);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Password).IsRequired().HasMaxLength(100);
            builder.Property(u => u.HashPassword).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Role).IsRequired().HasMaxLength(20);

            builder.Property(u => u.CreatedDate).IsRequired();
            builder.Property(u => u.ModifiedDate);

            builder.Property(u => u.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.HasQueryFilter(u => !u.IsDeleted);

        }
    }
}
