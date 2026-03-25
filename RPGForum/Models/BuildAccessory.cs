namespace RPGForum.Models
{
    public class BuildAccessory
    {
        public int BuildId { get; set; }
        public int AccessoryId { get; set; }

        public int SlotPosition { get; set; } = 1;

        public BuildPost Build { get; set; } = null!;
        public Acessórios Accessory { get; set; } = null!;
    }
}
