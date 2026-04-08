namespace RPGForum.Models
{
    public class BuildWeapon
    {
        public int BuildId { get; set; }
        public int WeaponId { get; set; }
        public int SlotPosition { get; set; } = 1;
        public BuildPost Build { get; set; } = null!;
        public Armas Weapon { get; set; } = null!;
    }
}
