using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPGForum.Models
{
    public class BuildAccessory
    {
        /// <summary>
        /// ID do build
        /// </summary>
        public int BuildId { get; set; }

        /// <summary>
        /// ID do acessório
        /// </summary>
        public int AccessoryId { get; set; }

        /// <summary>
        /// Posição do acessório no build
        /// </summary>
        public int SlotPosition { get; set; } = 1;

        public Build Build { get; set; } = null!;

        public Acessorios Accessory { get; set; } = null!;
    }
}
