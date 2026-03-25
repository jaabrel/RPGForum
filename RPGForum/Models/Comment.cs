using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPGForum.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [ForeignKey(nameof(BuildPost))]
        public int BuildId { get; set; }

        [ForeignKey(nameof(Utilizadores))]
        public int UserId { get; set; }

        [Required, MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public BuildPost Build { get; set; } = null!;
        public Utilizadores User { get; set; } = null!;
        public Comment? Parent { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();



    }
}
