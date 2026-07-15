using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;

namespace RPGForum.Pages.Admin.Personagens
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
        public Models.Personagens Personagens { get; set; } = default!;

        public int BuildsCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personagens = await _context.Personagens.FirstOrDefaultAsync(m => m.Id == id);
            if (personagens == null)
            {
                return NotFound();
            }
            Personagens = personagens;

            // Contar builds associadas para validação
            BuildsCount = await _context.Builds.CountAsync(b => b.CharacterId == id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personagens = await _context.Personagens.FindAsync(id);
            if (personagens != null)
            {
                Personagens = personagens;

                // Validação de exclusão: verificar se está em uso
                var buildsCount = await _context.Builds.CountAsync(b => b.CharacterId == id);
                if (buildsCount > 0)
                {
                    ModelState.AddModelError("", $"Não é possível eliminar a personagem \"{personagens.Name}\" porque ela está associada a {buildsCount} builds. Elimine ou atualize essas builds primeiro.");
                    BuildsCount = buildsCount;
                    return Page();
                }

                _context.Personagens.Remove(Personagens);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = $"Personagem \"{Personagens.Name}\" eliminada com sucesso!";
            }

            return RedirectToPage("./Index");
        }
    }
}
