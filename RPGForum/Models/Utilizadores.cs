using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPGForum.Models
{
    public class Utilizadores
    {
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(150), EmailAddressAttribute]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Role {  get; set; } = "Registered";

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<BuildPost> Builds { get; set; } = new List<BuildPost>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();


    }
}
