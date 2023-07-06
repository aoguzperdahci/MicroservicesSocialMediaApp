using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserService.Entities
{
    public class User
    {
        [Key]
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; }
        [JsonIgnore]
        public string PasswordSalt { get; set; }
        public string ProfilePhoto { get; set; }
    }
}
