using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Application.Models.Category.DTO
{
    public class CategorySaveDTO
    {
        public string Name { get; set; }
        public string UrlSlug { get; set; }
        public string Description { get; set; }
    }
}
