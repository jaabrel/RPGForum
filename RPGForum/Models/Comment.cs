using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPGForum.Models
{
    public class Comment
    {
        public int Id { get; set; }

        /// <summary>
        /// ID do build
        /// </summary>
        [ForeignKey(nameof(Build))]
        public int BuildId { get; set; }

        /// <summary>
        /// ID do utilizador
        /// </summary>
        [ForeignKey(nameof(Utilizadores))]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// ID do comentário pai (para respostas)
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Conteúdo do comentário
        /// </summary>
        [Required, MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Data de criação do comentário
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Build Build { get; set; } = null!;
        public Utilizadores User { get; set; } = null!;
        public Comment? Parent { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
