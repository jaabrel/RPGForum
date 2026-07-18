using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPGForum.Models
{
    public class Acessorios
    {
        public int Id { get; set; }

        /// <summary>
        /// Nome do acessório
        /// </summary>
        [Required(ErrorMessage = "O nome do acessório é demasiado grande")] 
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do acessório
        /// </summary>
        [MaxLength(100, ErrorMessage = "O nome do tipo do acessório é demasiado grande")]
        public string? Type { get; set; }

        /// <summary>
        /// Descrição do acessório
        /// </summary>
        [MaxLength(250, ErrorMessage = "A descrição do acessório é demasiado grande")]
        public string? Description { get; set; }

        /// <summary>
        /// URL da imagem do acessório
        /// </summary>
        [MaxLength(512)]
        public string? ImageUrl { get; set; }

        public string? StatAfetada { get; set; }
        public string? StatBonus { get; set; }

        public ICollection<BuildAccessory> BuildAccessories { get; set; } = new List<BuildAccessory>();
    }
}