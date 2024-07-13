using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Postable.Entities.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(u => u.Username).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();
                
            builder.Property(u => u.Username)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.Password)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(100);

            builder.Property(u => u.FirstName)
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .HasMaxLength(50);

            builder.Property(u => u.Role)
                .HasMaxLength(20)
                .IsRequired()
                .HasDefaultValue("user");

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.HasData(
                new User
                {   
                    Id = 1,
                    Username = "admin",
                    Password = "admin123",
                    Email = "admin@example.com",
                    Role = "admin"
                },
                new User
                {
                    Id = 2,
                    Username = "user1",
                    Password = "user123",
                    Email = "user1@example.com",
                    Role = "user"
                }
            );
        }
    }
}