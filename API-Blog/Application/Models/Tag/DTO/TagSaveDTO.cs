using System.ComponentModel.DataAnnotations;

namespace Application.Models.Tag.DTO
{
    public class TagSaveDTO
    {
        public string Name { get; set; }
        public string UrlSlug { get; set; }
        public string Description { get; set; }
    }
}
