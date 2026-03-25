using System.ComponentModel.DataAnnotations.Schema;

namespace RPGForum.Models
{
    public class Like
    {
        public int Id { get; set; }

        [ForeignKey(nameof(BuildPost))]
        public int BuildId { get; set; }

        [ForeignKey(nameof(Utilizadores))]
        public int UserId { get; set; }

        public DateTime CretatedAt { get; set; } = DateTime.UtcNow;

        public BuildPost Build { get; set; } = null!;
        public Utilizadores User { get; set; } = null!;
    }
}
