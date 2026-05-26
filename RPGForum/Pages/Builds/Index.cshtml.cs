using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;

namespace RPGForum.Pages.Builds
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<BuildPost> Builds { get; set; } = new List<BuildPost>();
        public string? MensagemSucesso { get; set; }

        public async Task<IActionResult> OnGetAsync() 
        {
            var utilizador = await ObterUtilizadorAtualAsync();
            if (utilizador == null) return RedirectToPage("/Index");

            if (TempData["Sucesso"] is string msg)
                MensagemSucesso = msg;

            Builds = await _context.Builds
                .Where(b => b.UtilizadorID == utilizador.Id)
                .Include(b => b.CharClass)
                .Include(b => b.Likes)
                .Include(b => b.Comments)
                .OrderByDescending(b => b.UpdatedAt)
                .ToListAsync();

            return Page();
        }

        /// <summary>
        /// Obtém ou cria o registo Utilizadores para o utilizador Identity atual.
        /// </summary>
        private async Task<Utilizadores?> ObterUtilizadorAtualAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return null;

            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.IdentityUserName == identityUser.UserName);

            if (utilizador == null)
            {
                // Criar registo na tabela Utilizadores se ainda não existir
                utilizador = new Utilizadores
                {
                    Username = identityUser.UserName!.Split('@')[0],
                    Email = identityUser.Email!,
                    Password = "", // Gerido pelo Identity
                    IdentityUserName = identityUser.UserName,
                    Role = "Registered"
                };
                _context.Utilizadores.Add(utilizador);
                await _context.SaveChangesAsync();
            }

            return utilizador;
        }

    }
}
