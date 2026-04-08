using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Pkcs;

namespace RPGForum.Models
{
    public class BuildPost
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Utilizadores))]
        public int UserId { get; set; }

        [ForeignKey(nameof(Personagens))]
        public int CharacterId { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2500)]
        public string? Description { get; set; }

        [Range(1, 100)]
        public int Level { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Utilizadores User { get; set; } = null!;
        public Personagens CharClass { get; set; } = null!;
        public Estatisticas Stats { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<BuildWeapon> BuidWeapons { get; set; } = new List<BuildWeapon>();
        public ICollection<BuildAccessory> BuildAccessories { get; set; } = new List<BuildAccessory>();




    }
}
