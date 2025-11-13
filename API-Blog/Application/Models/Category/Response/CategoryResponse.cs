
namespace Application.Models.Category.Response
{
    public class CategoryResponse
    {
        public bool Ok { get; set; } = true;
        public int Id { get; set; }
        public string Name { get; set; }
        public string UrlSlug { get; set; }
        public string Description { get; set; }
    }
}
