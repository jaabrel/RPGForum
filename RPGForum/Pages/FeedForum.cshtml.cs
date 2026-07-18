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

        public IList<BuildPost> Builds { get; set; } = new List<BuildPost>();
        public IList<Personagens> Personagens { get; set; } = new List<Personagens>();
        public IList<Utilizadores> Utilizadores { get; set; } = new List<Utilizadores>();

        [BindProperty(SupportsGet = true)]
        public string? Pesquisa { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PersonagemId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? UtilizadorID { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Ordenar { get; set; } = "recente";

        [BindProperty(SupportsGet = true)]
        public int Pagina { get; set; } = 1;

        public int TotalPaginas { get; set; }
        public int PaginaAtual { get; set; }
        public bool TemPaginaAnterior => PaginaAtual > 1;
        public bool TemProximaPagina => PaginaAtual < TotalPaginas;

        public async Task OnGetAsync()
        {
            // Carregar personagens para o filtro
            Personagens = await _context.Personagens
                .OrderBy(p => p.Name)
                .ToListAsync();

            // Carregar utilizadores para o filtro
            Utilizadores = await _context.Utilizadores
                .OrderBy(u => u.Username)
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

            // Filtro por utilizador
            if (UtilizadorID.HasValue)
            {
                query = query.Where(b => b.UtilizadorID == UtilizadorID.Value);
            }

            // Ordenação
            query = Ordenar switch
            {
                "popular" => query.OrderByDescending(b => b.Likes.Count),
                "nivel" => query.OrderByDescending(b => b.Level),
                _ => query.OrderByDescending(b => b.CreatedAt) // "recente"
            };

            // Contar total de itens para paginação
            int totalItems = await query.CountAsync();
            
            // Garantir valores válidos para página
            if (Pagina < 1) Pagina = 1;
            PaginaAtual = Pagina;
            TotalPaginas = (int)Math.Ceiling(totalItems / 30.0);
            if (TotalPaginas == 0) TotalPaginas = 1;
            if (PaginaAtual > TotalPaginas) PaginaAtual = TotalPaginas;

            // Executar query paginada (máximo 30 itens)
            Builds = await query
                .Skip((PaginaAtual - 1) * 30)
                .Take(30)
                .ToListAsync();
        }
    }

}
