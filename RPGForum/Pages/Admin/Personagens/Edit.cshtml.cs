using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;

namespace RPGForum.Pages.Admin.Personagens
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Personagens Personagem { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personagem = await _context.Personagens.FirstOrDefaultAsync(m => m.Id == id);
            if (personagem == null)
            {
                return NotFound();
            }
            Personagem = personagem;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Personagem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonagemExists(Personagem.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["Sucesso"] = $"Personagem \"{Personagem.Name}\" atualizada com sucesso!";
            return RedirectToPage("./Index");
        }

        private bool PersonagemExists(int id)
        {
            return _context.Personagens.Any(e => e.Id == id);
        }
    }
}
