using System.ComponentModel.DataAnnotations;

namespace RPGForum.Models
{
    public class Personagens
    {

        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(512)]
        public string? ImageUrl { get; set; }

        public ICollection<Build> Builds { get; set; } = new List<Build>();

    }
}
