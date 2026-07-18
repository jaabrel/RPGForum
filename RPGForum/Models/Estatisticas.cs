using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPGForum.Models
{
    public class Estatisticas
    {

        public int Id { get; set; }

        /// <summary>
        /// ID do build
        /// </summary>
        [ForeignKey(nameof(Build))]
        public int BuildId { get; set; }

        /// <summary>
        /// Health Points do Personagem
        /// </summary>
        public int Hp { get; set; } = 10;

        /// <summary>
        /// Strength do Personagem
        /// </summary>
        public int Strength { get; set; } = 10;
        
        /// <summary>
        /// Defense do Personagem
        /// </summary>
        public int Defense { get; set; } = 10;
        
        /// <summary>
        /// Magic do Personagem
        /// </summary>
        public int Magic { get; set; } = 10;
        
        /// <summary>
        /// Endurance do Personagem
        /// </summary>
        public int Endurance { get; set; } = 10;
        
        /// <summary>
        /// Speed do Personagem
        /// </summary>
        public int Speed { get; set; } = 10;

        public Build Build { get; set; } = null!;

    }
}
