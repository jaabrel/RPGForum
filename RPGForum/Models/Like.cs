using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPGForum.Models
{
    public class Like
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
        /// Data de criação do like
        /// </summary>
        public DateTime CretatedAt { get; set; } = DateTime.UtcNow;

        public Build Build { get; set; } = null!;
        public Utilizadores User { get; set; } = null!;
    }
}
