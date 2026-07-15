using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;

namespace RPGForum.Pages.Admin.Acessorios;

[Authorize(Roles = "Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Acessorios Acessorio { get; set; } = default!;

        public int BuildsCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var acessorio = await _context.Acessorios.FirstOrDefaultAsync(m => m.Id == id);
            if (acessorio == null)
            {
                return NotFound();
            }
            Acessorio = acessorio;

            // Contar builds associadas para validação
            BuildsCount = await _context.BuildAccessories.CountAsync(ba => ba.AccessoryId == id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var acessorio = await _context.Acessorios.FindAsync(id);
            if (acessorio != null)
            {
                Acessorio = acessorio;

                // Validação de exclusão: verificar se está em uso em alguma build
                var buildsCount = await _context.BuildAccessories.CountAsync(ba => ba.AccessoryId == id);
                if (buildsCount > 0)
                {
                    ModelState.AddModelError("", $"Não é possível eliminar o acessório \"{acessorio.Name}\" porque está associado a {buildsCount} builds. Remova o acessório dessas builds primeiro.");
                    BuildsCount = buildsCount;
                    return Page();
                }

                _context.Acessorios.Remove(Acessorio);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = $"Acessório \"{Acessorio.Name}\" eliminado com sucesso!";
            }

            return RedirectToPage("./Index");
        }
    }
