using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RPGForum.Data;

namespace RPGForum.Pages.Admin.Personagens
{
    [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Models.Personagens Personagem { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Personagens.Add(Personagem);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Personagem \"{Personagem.Name}\" criada com sucesso!";
            return RedirectToPage("./Index");
        }
    }
}
