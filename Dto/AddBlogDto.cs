using System.ComponentModel.DataAnnotations;

namespace Active_Blog_Service_API.Dto
{
    public class AddBlogDto
    {
        public string Title { get; set; }
        [MaxLength(50)]
        [MinLength(3)]
        public string Category { get; set; }
        public IFormFile Image { get; set; }
        [MinLength(50)]
        public string BlogContent { get; set; }
    }

}
