using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;

namespace RPGForum.Pages.Admin.Armas
{
    [Authorize(Roles = "Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Armas Arma { get; set; } = default!;

        public int BuildsCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arma = await _context.Armas.FirstOrDefaultAsync(m => m.Id == id);
            if (arma == null)
            {
                return NotFound();
            }
            Arma = arma;

            // Contar builds associadas para validação
            BuildsCount = await _context.BuildWeapons.CountAsync(bw => bw.WeaponId == id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arma = await _context.Armas.FindAsync(id);
            if (arma != null)
            {
                Arma = arma;

                // Validação de exclusão: verificar se está em uso em alguma build
                var buildsCount = await _context.BuildWeapons.CountAsync(bw => bw.WeaponId == id);
                if (buildsCount > 0)
                {
                    ModelState.AddModelError("", $"Não é possível eliminar a arma \"{arma.Name}\" porque ela está associada a {buildsCount} builds. Remova a arma dessas builds primeiro.");
                    BuildsCount = buildsCount;
                    return Page();
                }

                _context.Armas.Remove(Arma);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = $"Arma \"{Arma.Name}\" eliminada com sucesso!";
            }

            return RedirectToPage("./Index");
        }
    }
}
