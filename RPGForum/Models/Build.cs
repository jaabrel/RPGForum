using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Pkcs;

namespace RPGForum.Models
{
    public class Build
    {
        public int Id { get; set; }

        /// <summary>
        /// ID do utilizador que criou o build
        /// </summary>
        [ForeignKey(nameof(Utilizadores))] 
        public string UtilizadorID { get; set; } = string.Empty;

        /// <summary>
        /// ID da personagem que vai ser usada no build
        /// </summary>
        [ForeignKey(nameof(Personagens))]
        public int CharacterId { get; set; }

        /// <summary>
        /// Nome da build
        /// </summary>
        [Required(ErrorMessage = "O título é obrigatório.")] 
        [MaxLength(50, ErrorMessage = "O título não pode ter mais de 50 caracteres.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do build
        /// </summary>
        [MaxLength(2500, ErrorMessage = "A descrição não pode exceder os 2500 caracteres.")]
        public string? Description { get; set; }

        /// <summary>
        /// Nível da personagem
        /// </summary>
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
