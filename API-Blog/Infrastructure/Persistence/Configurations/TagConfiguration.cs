using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.Property(t => t.Name).IsRequired().HasMaxLength(255);
            builder.Property(t => t.UrlSlug).IsRequired();
            builder.HasIndex(t => t.UrlSlug).IsUnique();
            builder.Property(t => t.Description).HasMaxLength(1000);

            builder.HasMany(t => t.PostTags)
                   .WithOne(pt => pt.Tag)
                   .HasForeignKey(pt => pt.TagId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new Tag { Id = 1, Name = "AI", UrlSlug = "ai", Description = "Trí tuệ nhân tạo" },
                new Tag { Id = 2, Name = "Machine Learning", UrlSlug = "machine-learning", Description = "Học máy và ứng dụng" },
                new Tag { Id = 3, Name = "Startup", UrlSlug = "startup", Description = "Khởi nghiệp sáng tạo" },
                new Tag { Id = 4, Name = "Blockchain", UrlSlug = "blockchain", Description = "Công nghệ chuỗi khối" },
                new Tag { Id = 5, Name = "Văn hóa", UrlSlug = "van-hoa", Description = "Bài viết về văn hóa và xã hội" }
            );
        }
    }
}
