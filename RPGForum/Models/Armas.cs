using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPGForum.Models
{
    public class Armas
    {
        public int Id { get; set; }

        /// <summary>
        /// Nome da arma
        /// </summary>
        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Tipo da arma
        /// </summary>
        [MaxLength(50)]
        public string? Type { get; set; }

        /// <summary>
        /// Descrição da arma
        /// </summary>
        [MaxLength (250)]
        public string? Description { get; set; }

        /// <summary>
        /// URL da imagem da arma
        /// </summary>
        public string? ImageUrl { get; set; }

        public ICollection<BuildWeapon> BuildWeapons { get; set; } = new List<BuildWeapon>();

    
    }
}
