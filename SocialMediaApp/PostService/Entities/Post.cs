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


        public int UserId { get; set; }


        public string Description { get; set; }


        public string Imagepath { get; set; }


        public DateTime Ts { get; set; }


        public bool Published { get; set; }
    }
}
