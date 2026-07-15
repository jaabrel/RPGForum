using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;
using System.ComponentModel.DataAnnotations;

namespace RPGForum.Pages.Builds
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- Dados da página ---
        public BuildPost Build { get; set; } = null!;
        public bool IsAutor { get; set; }
        public bool JaDeuGosto { get; set; }
        public Utilizadores? UtilizadorAtual { get; set; }

        // --- Formulário de comentário ---
        [BindProperty]
        [Required(ErrorMessage = "O comentário não pode estar vazio.")]
        [MaxLength(2000, ErrorMessage = "O comentário não pode ter mais de 2000 caracteres.")]
        public string Conteudo { get; set; } = string.Empty;

        // Id do comentário pai (para respostas); null = comentário raiz
        [BindProperty]
        public int? CommentParentId { get; set; }

        // --- GET ---
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var build = await CarregarBuildAsync(id);
            if (build == null) return NotFound();

            Build = build;
            await PreencherDadosUtilizadorAsync();
            return Page();
        }

        // --- POST: Dar / Retirar Gosto ---
        // Removido [Authorize] do handler (Aviso MVC1001 resolvido). A segurança é validada imperativamente.
        public async Task<IActionResult> OnPostGostoAsync(int id)
        {
            var utilizador = await ObterUtilizadorAtualAsync();
            if (utilizador == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            var build = await _context.Builds
                .Include(b => b.Likes)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (build == null) return NotFound();

            var gostoExistente = build.Likes.FirstOrDefault(l => l.UserId == utilizador.Id);

            if (gostoExistente != null)
            {
                // Já deu gosto — retirar
                _context.Gostos.Remove(gostoExistente);
            }
            else
            {
                // Ainda não deu gosto — adicionar
                _context.Gostos.Add(new Like
                {
                    BuildId = id,
                    UserId = utilizador.Id
                });
            }

            await _context.SaveChangesAsync();

            // Redirecionar de volta para os detalhes
            return RedirectToPage(new { id });
        }

        // --- POST: Comentar (novo ou resposta) ---
        // Removido [Authorize] do handler (Aviso MVC1001 resolvido). A segurança é validada imperativamente.
        public async Task<IActionResult> OnPostComentarAsync(int id)
        {
            var utilizador = await ObterUtilizadorAtualAsync();
            if (utilizador == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            // Validar apenas o campo de conteúdo
            if (!ModelState.IsValid)
            {
                var build = await CarregarBuildAsync(id);
                if (build == null) return NotFound();
                Build = build;
                await PreencherDadosUtilizadorAsync();
                return Page();
            }

            // Validar que a build existe
            var buildExiste = await _context.Builds.AnyAsync(b => b.Id == id);
            if (!buildExiste) return NotFound();

            // Se for resposta, validar que o comentário pai existe e pertence a esta build
            if (CommentParentId.HasValue)
            {
                var pai = await _context.Comentario
                    .FirstOrDefaultAsync(c => c.Id == CommentParentId.Value && c.BuildId == id);
                if (pai == null) CommentParentId = null; // Pai inválido → tratar como raiz
            }

            var comentario = new Comment
            {
                BuildId = id,
                UserId = utilizador.Id,
                ParentId = CommentParentId,
                Content = Conteudo.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Comentario.Add(comentario);
            await _context.SaveChangesAsync();

            // Redirecionar de volta à página com âncora para os comentários
            return RedirectToPage(pageName: null, pageHandler: null, new { id }, fragment: "comentarios");
        }

        // --- POST: Apagar Comentário ---
        // Removido [Authorize] do handler (Aviso MVC1001 resolvido). A segurança é validada imperativamente.
        public async Task<IActionResult> OnPostApagarComentarioAsync(int id, int comentarioId)
        {
            var utilizador = await ObterUtilizadorAtualAsync();
            if (utilizador == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            var comentario = await _context.Comentario
                .FirstOrDefaultAsync(c => c.Id == comentarioId && c.BuildId == id);

            if (comentario == null) return NotFound();

            // Só o autor do comentário ou administrador pode apagar
            if (comentario.UserId != utilizador.Id && !User.IsInRole("Administrator"))
                return Forbid();

            _context.Comentario.Remove(comentario);
            await _context.SaveChangesAsync();

            return RedirectToPage(pageName: null, pageHandler: null, new { id }, fragment: "comentarios");
        }

        // --- Helpers privados ---

        private async Task<BuildPost?> CarregarBuildAsync(int id)
        {
            return await _context.Builds
                .Include(b => b.User)
                .Include(b => b.CharClass)
                .Include(b => b.Stats)
                .Include(b => b.Likes)
                .Include(b => b.BuidWeapons)
                    .ThenInclude(bw => bw.Weapon)
                .Include(b => b.BuildAccessories)
                    .ThenInclude(ba => ba.Accessory)
                .Include(b => b.Comments.Where(c => c.ParentId == null))
                    .ThenInclude(c => c.User)
                .Include(b => b.Comments.Where(c => c.ParentId == null))
                    .ThenInclude(c => c.Replies)
                        .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        private async Task PreencherDadosUtilizadorAsync()
        {
            if (!User.Identity!.IsAuthenticated) return;

            UtilizadorAtual = await ObterUtilizadorAtualAsync();
            if (UtilizadorAtual == null || Build == null) return;

            IsAutor = Build.UtilizadorID == UtilizadorAtual.Id;
            JaDeuGosto = Build.Likes.Any(l => l.UserId == UtilizadorAtual.Id);
        }

        private async Task<Utilizadores?> ObterUtilizadorAtualAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return null;

            return await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.IdentityUserName == identityUser.UserName);
        }
    }
}
