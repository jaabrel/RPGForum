using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPGForum.Models
{
    public class Personagens
    {

        public int Id { get; set; }

        /// <summary>
        /// Nome do personagem
        /// </summary>
        [Required, MaxLength(30)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do personagem
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// URL da imagem do personagem
        /// </summary>
        [MaxLength(512)]
        public string? ImageUrl { get; set; }

        public ICollection<Build> Builds { get; set; } = new List<Build>();

    }
}
