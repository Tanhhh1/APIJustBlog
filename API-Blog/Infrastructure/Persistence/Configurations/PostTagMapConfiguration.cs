using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PostTagMapConfiguration : IEntityTypeConfiguration<PostTagMap>
    {
        public void Configure(EntityTypeBuilder<PostTagMap> builder)
        {
            builder.HasKey(pt => new { pt.PostId, pt.TagId });

            builder.HasData(
                new PostTagMap { PostId = 1, TagId = 1 },
                new PostTagMap { PostId = 1, TagId = 2 },
                new PostTagMap { PostId = 2, TagId = 2 },
                new PostTagMap { PostId = 3, TagId = 3 },
                new PostTagMap { PostId = 4, TagId = 4 },
                new PostTagMap { PostId = 5, TagId = 5 }
            );
        }
    }
}
