using System.ComponentModel.DataAnnotations;

namespace RPGForum.Models
{
    public class Acessórios
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? Type { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }

        [MaxLength(512)]
        public string? ImageUrl { get; set; }

        public ICollection<BuildAccessory> BuildAccessories { get; set; } = new List<BuildAccessory>();

    }
}
