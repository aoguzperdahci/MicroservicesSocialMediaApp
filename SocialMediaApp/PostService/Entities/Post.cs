using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PostService.Entities
{
    [Table("Post")]
    public class Post
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public DateTime PublishTime { get; set; }
    }
}
