using System.ComponentModel.DataAnnotations;

namespace Active_Blog_Service_API.Dto
{
    public class ContactDto
    {
        [MaxLength(80)]
        [MinLength(3)]
        public string MessageTitle { get; set; }
        [MinLength(3)]
        public string MessageBody { get; set; }
    }
}
