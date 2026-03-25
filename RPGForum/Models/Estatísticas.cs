using System.ComponentModel.DataAnnotations.Schema;

namespace RPGForum.Models
{
    public class Estatísticas
    {

        public int Id { get; set; }

        [ForeignKey(nameof(BuildPost))]
        public int BuildId { get; set; }

        public int Hp { get; set; } = 10;
        public int Strength { get; set; } = 10;
        public int Defense { get; set; } = 10;
        public int Magic { get; set; } = 10;
        public int Endurance { get; set; } = 10;
        public int Speed { get; set; } = 10;



        public BuildPost Build { get; set; } = null!;

    }
}
