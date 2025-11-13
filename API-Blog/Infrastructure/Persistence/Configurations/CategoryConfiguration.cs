using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(c => c.Name).IsRequired().HasMaxLength(255);
            builder.Property(c => c.UrlSlug).IsRequired();
            builder.HasIndex(c => c.UrlSlug).IsUnique();
            builder.Property(c => c.Description).HasMaxLength(1000);

            builder.HasMany(c => c.Posts)
                   .WithOne(p => p.Category)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new Category { Id = 1, Name = "Công nghệ", UrlSlug = "cong-nghe", Description = "Tin tức công nghệ mới nhất" },
                new Category { Id = 2, Name = "Khoa học", UrlSlug = "khoa-hoc", Description = "Khám phá và nghiên cứu khoa học" },
                new Category { Id = 3, Name = "Giải trí", UrlSlug = "giai-tri", Description = "Tin tức về phim ảnh, âm nhạc" },
                new Category { Id = 4, Name = "Kinh tế", UrlSlug = "kinh-te", Description = "Tin tức tài chính và kinh doanh" },
                new Category { Id = 5, Name = "Thể thao", UrlSlug = "the-thao", Description = "Cập nhật các sự kiện thể thao mới nhất" }
            );
        }
    }
}
