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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



    }
}
