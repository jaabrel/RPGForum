using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPGForum.Models
{
    public class Utilizadores : IdentityUser
    {
        /// <summary>
        /// Role do Utilizador
        /// </summary>
        [MaxLength(20)]
        public string Role { get; set; } = "Registered";

        /// <summary>
        /// Data de criação do Utilizador
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Build> Builds { get; set; } = new List<Build>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
