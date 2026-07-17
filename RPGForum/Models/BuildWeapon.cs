using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPGForum.Models
{
    public class BuildWeapon
    {
        /// <summary>
        /// ID do build
        /// </summary>
        public int BuildId { get; set; }

        /// <summary>
        /// ID da arma
        /// </summary>
        public int WeaponId { get; set; }

        /// <summary>
        /// Posição da arma no build
        /// </summary>
        public int SlotPosition { get; set; } = 1;

        public Build Build { get; set; } = null!;

        public Armas Weapon { get; set; } = null!;
    }
}
