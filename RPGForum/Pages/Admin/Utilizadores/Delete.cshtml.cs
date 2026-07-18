using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using System.Threading.Tasks;

namespace RPGForum.Pages.Admin.Utilizadores
{
    [Authorize(Roles = "Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Models.Utilizadores Utilizador { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(m => m.Id == id);

            if (utilizador == null)
            {
                return NotFound();
            }
            else
            {
                Utilizador = utilizador;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.Id == id);

            if (utilizador != null)
            {
                // Impedir que o administrador se elimine a si próprio
                if (utilizador.Email == User.Identity?.Name)
                {
                    ModelState.AddModelError(string.Empty, "Não podes apagar a tua própria conta de administrador!");
                    Utilizador = utilizador;
                    return Page();
                }

                Utilizador = utilizador;

                // Remover do ASP.NET Identity
                var identityUser = await _userManager.FindByEmailAsync(utilizador.Email);
                if (identityUser != null)
                {
                    await _userManager.DeleteAsync(identityUser);
                }

                // Remover da tabela Utilizadores (e em cascata builds/likes/comments se configurado)
                _context.Utilizadores.Remove(utilizador);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = $"Utilizador \"{utilizador.Username}\" eliminado com sucesso!";
            }

            return RedirectToPage("./Index");
        }
    }
}
