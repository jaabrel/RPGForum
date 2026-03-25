using System.ComponentModel.DataAnnotations;

namespace RPGForum.Models
{
    public class Armas
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Type { get; set; }
        [MaxLength (250)]
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public ICollection<BuildWeapon> BuildWeapons { get; set; } = new List<BuildWeapon>();

    
    }
}
