using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;

namespace RPGForum.Pages
{
    public class FeedForumModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public FeedForumModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Build> Builds { get; set; } = new List<Models.Build>();
        public IList<Personagens> Personagens { get; set; } = new List<Personagens>();

        [BindProperty(SupportsGet = true)]
        public string? Pesquisa { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PersonagemId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Ordenar { get; set; } = "recente";

        public async Task OnGetAsync()
        {
            // Carregar personagens para o filtro
            Personagens = await _context.Personagens
                .OrderBy(p => p.Name)
                .ToListAsync();

            // Construir query com LINQ
            var query = _context.Builds
                .Include(b => b.User)
                .Include(b => b.CharClass)
                .Include(b => b.Likes)
                .Include(b => b.Comments)
                .AsQueryable();

            // Filtro por pesquisa de texto
            if (!string.IsNullOrWhiteSpace(Pesquisa))
            {
                query = query.Where(b =>
                    b.Title.Contains(Pesquisa) ||
                    (b.Description != null && b.Description.Contains(Pesquisa)));
            }

            // Filtro por personagem/classe
            if (PersonagemId.HasValue)
            {
                query = query.Where(b => b.CharacterId == PersonagemId.Value);
            }

            // Ordenação
            query = Ordenar switch
            {
                "popular" => query.OrderByDescending(b => b.Likes.Count),
                "nivel" => query.OrderByDescending(b => b.Level),
                _ => query.OrderByDescending(b => b.CreatedAt) // "recente"
            };

            Builds = await query.ToListAsync();
        }
    }

}
