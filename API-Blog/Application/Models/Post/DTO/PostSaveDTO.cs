using System.ComponentModel.DataAnnotations;


namespace Application.Models.Post.DTO
{
    public class PostSaveDTO
    {
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Meta { get; set; }
        public string UrlSlug { get; set; }
        public bool Published { get; set; }
    }
}
