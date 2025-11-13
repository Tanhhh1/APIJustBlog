using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property(p => p.Title).IsRequired().HasMaxLength(255);
            builder.Property(p => p.UrlSlug).IsRequired().HasMaxLength(450);
            builder.HasIndex(p => p.UrlSlug).IsUnique();

            builder.HasMany(p => p.PostTags)
                   .WithOne(pt => pt.Post)
                   .HasForeignKey(pt => pt.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new Post
                {
                    Id = 1,
                    Title = "AI thay đổi thế giới như thế nào",
                    ShortDescription = "Cách AI tác động đến các ngành nghề trong tương lai.",
                    Description = "AI đang dần trở thành một phần không thể thiếu...",
                    Meta = "AI, công nghệ, tương lai",
                    UrlSlug = "ai-thay-doi-the-gioi",
                    Published = true,
                    PostedOn = new DateTime(2025, 11, 6),
                    CategoryId = 1
                },
                new Post
                {
                    Id = 2,
                    Title = "Khám phá vũ trụ với kính viễn vọng mới",
                    ShortDescription = "Các nhà khoa học đã phát hiện hành tinh mới...",
                    Description = "Kính viễn vọng James Webb giúp con người nhìn xa hơn...",
                    Meta = "khoa học, vũ trụ",
                    UrlSlug = "vu-tru-voi-kinh-vien-vong-moi",
                    Published = true,
                    PostedOn = new DateTime(2025, 12, 6),
                    CategoryId = 2
                },
                new Post
                {
                    Id = 3,
                    Title = "Doanh nghiệp Việt và làn sóng Startup",
                    ShortDescription = "Khởi nghiệp đang là xu hướng mạnh tại Việt Nam.",
                    Description = "Nhiều startup công nghệ ra đời, thu hút vốn đầu tư lớn...",
                    Meta = "startup, kinh tế",
                    UrlSlug = "doanh-nghiep-viet-startup",
                    Published = true,
                    PostedOn = new DateTime(2025, 11, 4),
                    CategoryId = 4
                },
                new Post
                {
                    Id = 4,
                    Title = "Xu hướng Blockchain trong tài chính",
                    ShortDescription = "Blockchain đang thay đổi cách giao dịch tài chính.",
                    Description = "Công nghệ chuỗi khối mang lại sự minh bạch và bảo mật...",
                    Meta = "blockchain, tài chính",
                    UrlSlug = "blockchain-trong-tai-chinh",
                    Published = true,
                    PostedOn = new DateTime(2025, 6, 6),
                    CategoryId = 4
                },
                new Post
                {
                    Id = 5,
                    Title = "Olympic Paris 2024 - niềm tự hào thể thao Việt",
                    ShortDescription = "Việt Nam giành huy chương tại Olympic Paris.",
                    Description = "Các vận động viên Việt Nam đạt thành tích ấn tượng...",
                    Meta = "thể thao, olympic",
                    UrlSlug = "olympic-paris-2024",
                    Published = true,
                    PostedOn = new DateTime(2025, 1, 6),
                    CategoryId = 5
                }
            );
        }
    }
}
