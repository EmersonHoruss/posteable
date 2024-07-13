using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postable.Entities;

namespace Postable.Entities.Configurations
{
    public class PostConfig : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property(p => p.UserId).IsRequired();

            builder.Property(p => p.Content)
                    .HasMaxLength(500)
                    .IsRequired();

            builder.Property(p => p.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETDATE()");

            builder.HasOne(p => p.User)
                   .WithMany(u => u.Posts)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new Post { Id = 1, UserId = 1, Content = "First post!" },
                new Post { Id = 2, UserId = 2, Content = "Hello world!" }
            );
        }
    }
}
