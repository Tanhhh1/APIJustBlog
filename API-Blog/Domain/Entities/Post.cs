using Domain.Common;

namespace Domain.Entities
{
    public class Post : BaseEntity
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Meta { get; set; }
        public string UrlSlug { get; set; }
        public bool Published { get; set; }
        public DateTime PostedOn { get; set; } 
        public DateTime? Modified { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<PostTagMap> PostTags { get; set; } = new List<PostTagMap>();
    }
}