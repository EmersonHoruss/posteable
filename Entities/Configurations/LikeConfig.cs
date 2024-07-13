using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postable.Entities;

namespace Postable.Entities.Configurations
{
    public class LikeConfig : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.Property(l => l.PostId).IsRequired();
            
            builder.Property(l => l.UserId).IsRequired();

            builder.Property(l => l.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETDATE()");

            builder.HasOne(l => l.Post)
                   .WithMany(p => p.Likes)
                   .HasForeignKey(l => l.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.User)
                   .WithMany(u => u.Likes)
                   .HasForeignKey(l => l.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(l => new { l.PostId, l.UserId }).IsUnique();

            builder.HasData(
                new Like { Id = 1, PostId = 1, UserId = 1 },
                new Like { Id = 2, PostId = 2, UserId = 2 }
            );
        }
    }
}