using Domain.Common;

namespace Domain.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public string UrlSlug { get; set; }
        public string Description { get; set; }
        public ICollection<PostTagMap> PostTags { get; set; } = new List<PostTagMap>();
    }
}
