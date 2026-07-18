using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPGForum.Pages.Build
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.Utilizadores> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<Models.Utilizadores> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Models.Build> Builds { get; set; } = new List<Models.Build>();
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
                .Include(b => b.Stats)
                .Include(b => b.Likes)
                .Include(b => b.Comments)
                .OrderByDescending(b => b.UpdatedAt)
                .ToListAsync();

            return Page();
        }

        private async Task<Models.Utilizadores?> ObterUtilizadorAtualAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}
