using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Post.Response
{
    public class PostResponse
    {
        public bool Ok { get; set; } = true;
        public int Id { get; set; }
        public string Title { get; set; }
        public string UrlSlug { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public bool Published { get; set; }
        public DateTime PostedOn { get; set; }
        public DateTime? Modified { get; set; }
        
    }
}
