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
        private readonly UserManager<Models.Utilizadores> _userManager;

        public DeleteModel(ApplicationDbContext context, UserManager<Models.Utilizadores> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Models.Utilizadores Utilizador { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
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

        public async Task<IActionResult> OnPostAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.Id == id);

            if (utilizador != null)
            {
                // Impedir que o administrador se elimine a si próprio
                if (utilizador.Id == _userManager.GetUserId(User))
                {
                    ModelState.AddModelError(string.Empty, "Não podes apagar a tua própria conta de administrador!");
                    Utilizador = utilizador;
                    return Page();
                }

                Utilizador = utilizador;

                // Remover do ASP.NET Identity (que já remove da base de dados e trata de cascata)
                var result = await _userManager.DeleteAsync(utilizador);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                TempData["Sucesso"] = $"Utilizador \"{utilizador.UserName}\" eliminado com sucesso!";
            }

            return RedirectToPage("./Index");
        }
    }
}
