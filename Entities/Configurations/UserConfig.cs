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
                .IsRequired();

            builder.Property(u => u.Password)
                .IsRequired();

            builder.Property(u => u.Role)
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